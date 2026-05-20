using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProxmoxVEGui
{
    public class ProxmoxClient
    {
        private readonly HttpClient _httpClient;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Ticket { get; private set; }
        public string CsrfToken { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public ProxmoxClient(string host, int port, bool ignoreSsl)
        {
            Host = host;
            Port = port;

            var handler = new HttpClientHandler
            {
                UseCookies = false
            };
            if (ignoreSsl)
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                // Enable modern TLS protocols
                handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            }

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri($"https://{host}:{port}/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> LoginAsync(string username, string password, string realm)
        {
            Username = username;
            if (!Username.Contains("@"))
            {
                Username += "@" + realm;
            }

            var parameters = new Dictionary<string, string>
            {
                { "username", Username },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(parameters);
            try
            {
                var response = await _httpClient.PostAsync("api2/json/access/ticket", content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                Ticket = data["ticket"]?.ToString();
                CsrfToken = data["CSRFPreventionToken"]?.ToString();

                if (string.IsNullOrEmpty(Ticket) || string.IsNullOrEmpty(CsrfToken))
                {
                    return false;
                }

                // Configure Client with Headers
                _httpClient.DefaultRequestHeaders.Remove("Cookie");
                _httpClient.DefaultRequestHeaders.Add("Cookie", $"PVEAuthCookie={Ticket}");
                
                // Note: CSRF token is required for POST/PUT/DELETE
                _httpClient.DefaultRequestHeaders.Remove("CSRFPreventionToken");
                _httpClient.DefaultRequestHeaders.Add("CSRFPreventionToken", CsrfToken);

                IsAuthenticated = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<ClusterNode>> GetClusterStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api2/json/cluster/status");
                if (!response.IsSuccessStatusCode) return new List<ClusterNode>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var nodes = new List<ClusterNode>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        if (token["type"]?.ToString() == "node")
                        {
                            nodes.Add(new ClusterNode
                            {
                                Name = token["name"]?.ToString(),
                                NodeId = token["nodeid"]?.ToInt() ?? 0,
                                Ip = token["ip"]?.ToString(),
                                Online = token["online"]?.ToInt() == 1,
                                Level = token["level"]?.ToString()
                            });
                        }
                    }
                }
                return nodes;
            }
            catch
            {
                return new List<ClusterNode>();
            }
        }

        public async Task<List<PveNode>> GetNodesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api2/json/nodes");
                if (!response.IsSuccessStatusCode) return new List<PveNode>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var nodes = new List<PveNode>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        nodes.Add(new PveNode
                        {
                            Node = token["node"]?.ToString(),
                            Status = token["status"]?.ToString(),
                            Cpu = token["cpu"]?.ToDouble() ?? 0.0,
                            MaxCpu = token["maxcpu"]?.ToInt() ?? 1,
                            Mem = token["mem"]?.ToLong() ?? 0,
                            MaxMem = token["maxmem"]?.ToLong() ?? 1,
                            Disk = token["disk"]?.ToLong() ?? 0,
                            MaxDisk = token["maxdisk"]?.ToLong() ?? 1,
                            Uptime = token["uptime"]?.ToLong() ?? 0
                        });
                    }
                }
                return nodes;
            }
            catch
            {
                return new List<PveNode>();
            }
        }

        public async Task<List<PveVm>> GetVmsAsync(string node)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api2/json/nodes/{node}/qemu");
                if (!response.IsSuccessStatusCode) return new List<PveVm>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var vms = new List<PveVm>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        vms.Add(new PveVm
                        {
                            VmId = token["vmid"]?.ToInt() ?? 0,
                            Name = token["name"]?.ToString(),
                            Status = token["status"]?.ToString(),
                            Cpu = token["cpu"]?.ToDouble() ?? 0.0,
                            MaxCpu = token["cpus"]?.ToInt() ?? 1,
                            Mem = token["mem"]?.ToLong() ?? 0,
                            MaxMem = token["maxmem"]?.ToLong() ?? 1,
                            Uptime = token["uptime"]?.ToLong() ?? 0,
                            IsTemplate = token["template"]?.ToInt() == 1
                        });
                    }
                }
                return vms;
            }
            catch
            {
                return new List<PveVm>();
            }
        }

        public async Task<List<PveLxc>> GetLxcsAsync(string node)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api2/json/nodes/{node}/lxc");
                if (!response.IsSuccessStatusCode) return new List<PveLxc>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var lxcs = new List<PveLxc>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        lxcs.Add(new PveLxc
                        {
                            VmId = token["vmid"]?.ToInt() ?? 0,
                            Name = token["name"]?.ToString(),
                            Status = token["status"]?.ToString(),
                            Cpu = token["cpu"]?.ToDouble() ?? 0.0,
                            MaxCpu = token["cpus"]?.ToInt() ?? 1,
                            Mem = token["mem"]?.ToLong() ?? 0,
                            MaxMem = token["maxmem"]?.ToLong() ?? 1,
                            Uptime = token["uptime"]?.ToLong() ?? 0
                        });
                    }
                }
                return lxcs;
            }
            catch
            {
                return new List<PveLxc>();
            }
        }

        public async Task<List<PveStorage>> GetStorageAsync(string node)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api2/json/nodes/{node}/storage");
                if (!response.IsSuccessStatusCode) return new List<PveStorage>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var storages = new List<PveStorage>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        storages.Add(new PveStorage
                        {
                            Storage = token["storage"]?.ToString(),
                            Type = token["type"]?.ToString(),
                            Active = token["active"]?.ToInt() == 1,
                            Used = token["used"]?.ToLong() ?? 0,
                            Total = token["total"]?.ToLong() ?? 0
                        });
                    }
                }
                return storages;
            }
            catch
            {
                return new List<PveStorage>();
            }
        }

        public async Task<List<PveTask>> GetTasksAsync(string fallbackNode = null)
        {
            try
            {
                // Try cluster tasks first
                var response = await _httpClient.GetAsync("api2/json/cluster/tasks?limit=30");
                if (!response.IsSuccessStatusCode && !string.IsNullOrEmpty(fallbackNode))
                {
                    response = await _httpClient.GetAsync($"api2/json/nodes/{fallbackNode}/tasks?limit=30");
                }

                if (!response.IsSuccessStatusCode) return new List<PveTask>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var tasks = new List<PveTask>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        tasks.Add(new PveTask
                        {
                            Node = token["node"]?.ToString(),
                            User = token["user"]?.ToString(),
                            StartTime = token["starttime"]?.ToLong() ?? 0,
                            EndTime = token["endtime"]?.ToLong() ?? 0,
                            Type = token["type"]?.ToString(),
                            Id = token["id"]?.ToString(),
                            Status = token["status"]?.ToString() ?? "RUNNING"
                        });
                    }
                }
                return tasks;
            }
            catch
            {
                return new List<PveTask>();
            }
        }

        public async Task<string> GetLxcIpAsync(string node, int vmid)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api2/json/nodes/{node}/lxc/{vmid}/interfaces");
                if (!response.IsSuccessStatusCode) return "Unknown";

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];
                if (data != null)
                {
                    foreach (var eth in data)
                    {
                        var name = eth["name"]?.ToString();
                        if (name == "lo") continue;

                        var inet = eth["inet"]?.ToString();
                        if (!string.IsNullOrEmpty(inet))
                        {
                            if (inet.Contains("/"))
                            {
                                inet = inet.Split('/')[0];
                            }
                            return inet;
                        }
                    }
                }
                return "No IP Address";
            }
            catch
            {
                return "Unknown";
            }
        }

        public async Task<string> GetVmIpAsync(string node, int vmid)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api2/json/nodes/{node}/qemu/{vmid}/agent/network-get-interfaces");
                if (!response.IsSuccessStatusCode) return "Unknown (Agent offline)";

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];
                if (data != null)
                {
                    foreach (var eth in data)
                    {
                        var name = eth["name"]?.ToString();
                        if (name == "lo" || name == "loopback") continue;

                        var ips = eth["ip-addresses"];
                        if (ips != null)
                        {
                            foreach (var ipObj in ips)
                            {
                                var type = ipObj["ip-address-type"]?.ToString();
                                var addr = ipObj["ip-address"]?.ToString();
                                if (type == "ipv4" && !string.IsNullOrEmpty(addr))
                                {
                                    return addr;
                                }
                            }
                        }
                    }
                }
                return "No IPv4 (Agent Active)";
            }
            catch
            {
                return "Unknown (Agent offline)";
            }
        }

        public async Task<bool> VMActionAsync(string node, int vmid, string type, string action)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api2/json/nodes/{node}/{type}/{vmid}/status/{action}", new FormUrlEncodedContent(new Dictionary<string, string>()));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteResourceAsync(string node, int vmid, string type)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api2/json/nodes/{node}/{type}/{vmid}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateVmAsync(string node, int vmid, string name, int cores, int memoryMb, string bridge)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "vmid", vmid.ToString() },
                    { "name", name },
                    { "cores", cores.ToString() },
                    { "memory", memoryMb.ToString() },
                    { "net0", $"virtio,bridge={bridge}" },
                    { "scsihw", "virtio-scsi-pci" },
                    { "scsi0", "local-lvm:32,discard=on" },
                    { "ostype", "l26" }
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync($"api2/json/nodes/{node}/qemu", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateVmAdvancedAsync(string node, Dictionary<string, string> parameters)
        {
            try
            {
                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync($"api2/json/nodes/{node}/qemu", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateLxcAsync(string node, int vmid, string name, int cores, int memoryMb, string ostemplate, string bridge)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "vmid", vmid.ToString() },
                    { "hostname", name },
                    { "cores", cores.ToString() },
                    { "memory", memoryMb.ToString() },
                    { "ostemplate", ostemplate },
                    { "rootfs", "local-lvm:8" },
                    { "net0", $"name=eth0,bridge={bridge},ip=dhcp" }
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync($"api2/json/nodes/{node}/lxc", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateLxcAdvancedAsync(string node, Dictionary<string, string> parameters)
        {
            try
            {
                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync($"api2/json/nodes/{node}/lxc", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PveStorageContent>> GetStorageContentAsync(string node, string storage)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api2/json/nodes/{node}/storage/{storage}/content");
                if (!response.IsSuccessStatusCode) return new List<PveStorageContent>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"];

                var contents = new List<PveStorageContent>();
                if (data != null)
                {
                    foreach (var token in data)
                    {
                        contents.Add(new PveStorageContent
                        {
                            VolId = token["volid"]?.ToString(),
                            Format = token["format"]?.ToString(),
                            Size = token["size"]?.ToLong() ?? 0,
                            Content = token["content"]?.ToString()
                        });
                    }
                }
                return contents;
            }
            catch
            {
                return new List<PveStorageContent>();
            }
        }

        public async Task<bool> UploadFileAsync(string node, string storage, string contentType, string filePath)
        {
            try
            {
                using (var fileContent = new StreamContent(System.IO.File.OpenRead(filePath)))
                {
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(new StringContent(contentType), "content");
                        formData.Add(fileContent, "filename", System.IO.Path.GetFileName(filePath));

                        // Proxmox upload endpoint
                        var response = await _httpClient.PostAsync($"api2/json/nodes/{node}/storage/{storage}/upload", formData);
                        return response.IsSuccessStatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Fetches the current configuration of a VM or LXC container.
        /// Returns a flat dictionary of key→value from the Proxmox API /config endpoint.
        /// </summary>
        public async Task<Dictionary<string, object>> GetConfigAsync(string node, int vmid, string type)
        {
            try
            {
                string endpoint = type == "vm"
                    ? $"api2/json/nodes/{node}/qemu/{vmid}/config"
                    : $"api2/json/nodes/{node}/lxc/{vmid}/config";

                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)response.StatusCode}");

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JObject>(jsonString);
                var data = result["data"] as JObject;

                if (data == null) return new Dictionary<string, object>();

                var dict = new Dictionary<string, object>();
                foreach (var prop in data.Properties())
                {
                    dict[prop.Name] = prop.Value?.ToString();
                }
                return dict;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetConfigAsync failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a PUT request with updated configuration parameters to Proxmox.
        /// Proxmox uses PUT on /config to merge-update configuration.
        /// </summary>
        public async Task<bool> UpdateConfigAsync(string node, int vmid, string type, Dictionary<string, string> parameters)
        {
            try
            {
                string endpoint = type == "vm"
                    ? $"api2/json/nodes/{node}/qemu/{vmid}/config"
                    : $"api2/json/nodes/{node}/lxc/{vmid}/config";

                var content = new FormUrlEncodedContent(parameters);
                var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateConfigAsync failed: {ex.Message}");
                return false;
            }
        }
    }

    public static class TokenExtensions
    {
        public static int ToInt(this JToken token) => token != null && int.TryParse(token.ToString(), out int val) ? val : 0;
        public static long ToLong(this JToken token) => token != null && long.TryParse(token.ToString(), out long val) ? val : 0;
        public static double ToDouble(this JToken token) => token != null && double.TryParse(token.ToString(), out double val) ? val : 0.0;
    }

    public class ClusterNode
    {
        public string Name { get; set; }
        public int NodeId { get; set; }
        public string Ip { get; set; }
        public bool Online { get; set; }
        public string Level { get; set; }
    }

    public class PveNode
    {
        public string Node { get; set; }
        public string Status { get; set; }
        public double Cpu { get; set; }
        public int MaxCpu { get; set; }
        public long Mem { get; set; }
        public long MaxMem { get; set; }
        public long Disk { get; set; }
        public long MaxDisk { get; set; }
        public long Uptime { get; set; }
    }

    public class PveVm
    {
        public int VmId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double Cpu { get; set; }
        public int MaxCpu { get; set; }
        public long Mem { get; set; }
        public long MaxMem { get; set; }
        public long Uptime { get; set; }
        public bool IsTemplate { get; set; }
    }

    public class PveLxc
    {
        public int VmId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double Cpu { get; set; }
        public int MaxCpu { get; set; }
        public long Mem { get; set; }
        public long MaxMem { get; set; }
        public long Uptime { get; set; }
    }

    public class PveStorage
    {
        public string Storage { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public long Used { get; set; }
        public long Total { get; set; }
    }

    public class PveTask
    {
        public string Node { get; set; }
        public string User { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class PveStorageContent
    {
        public string VolId { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
        public string Content { get; set; }
    }
}
