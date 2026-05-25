# Proxmox-Verwaltung

Eine moderne Windows-Desktop-Anwendung zur Verwaltung von Proxmox VE über die Proxmox-API.

Die Anwendung bietet eine grafische Oberfläche für Nodes, virtuelle Maschinen, LXC-Container, Storage-Pools, Konsolen-Zugriff, Ressourcenaktionen und grundlegende Konfigurationen.

## Funktionen

- Anmeldung an einem Proxmox-VE-Host per Host/IP, Port, Benutzername, Passwort und Realm
- Unterstützung für `pam` und `pve` Realm
- Optionales Ignorieren selbstsignierter SSL-Zertifikate
- Speicherung mehrerer Hostprofile
- Laden, Löschen und Umbenennen gespeicherter Verbindungen
- Übersicht über Datacenter, Nodes, VMs, LXC-Container und Storage-Pools
- Moderne TreeView mit Statusanzeige für laufende und gestoppte Ressourcen
- CPU- und RAM-Anzeige über Diagramme
- Anzeige von Uptime, Status, Speicher, CPU und IP-Informationen
- Task-Log mit Statusanzeige
- Starten, Stoppen, Herunterfahren und Neustarten von VMs/LXC-Containern
- Löschen von VMs/LXC-Containern
- noVNC-Konsole für VMs
- Container-Shell für LXC
- Node-Shell für Proxmox-Nodes
- Erstellung neuer VMs über einen Wizard
- Erstellung neuer LXC-Container über einen Wizard
- Dynamische Auswahl von Storage, ISO-Dateien und Container-Templates
- Drag-and-drop Upload für ISO-Dateien und LXC-Templates
- Konfigurationspanel für VMs und LXC-Container
- Bearbeitung von allgemeinen Einstellungen, Hardware, Netzwerk, Boot-Optionen und LXC-Features

## Sicherheitshinweise

- Passwörter werden nicht in den gespeicherten Hostprofilen abgelegt.
- Bei aktivierter Option zum Ignorieren von SSL-Fehlern werden Zertifikatsfehler nicht geprüft.
- Diese Option sollte nur in vertrauenswürdigen internen Netzwerken verwendet werden.
- Löschaktionen sind dauerhaft und sollten nur mit Bedacht genutzt werden.

## Bekannte Hinweise

- Die noVNC-/Shell-Konsole benötigt WebView2.
- Für VM-IP-Adressen wird der QEMU Guest Agent benötigt.
- LXC-IP-Adressen werden über die Proxmox-Interface-API abgefragt.
- Je nach Proxmox-Berechtigungen können einzelne Aktionen fehlschlagen.
- Selbstsignierte Zertifikate können ohne aktivierte SSL-Ausnahme Login- oder Konsolenprobleme verursachen.

## Haftungsausschluss

Diese Anwendung ist ein Verwaltungswerkzeug für Proxmox VE.  
Die Nutzung erfolgt auf eigene Verantwortung.
