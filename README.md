# WirelessChessGlass

Jouez aux échecs en réalité augmentée.
Ce projet utilise la table Microsoft PixelSense (anciennement Surface) qui représente l'échiquier, et des lunettes de réalité augmentée (nous avons utilisé les Epson Moverio BT-200, sous Android) pour afficher les pièces adverses en réalité augmentée.

# Hiérarchie du projet


Le dossier [BLUETOOTH - ANDROID](BLUETOOTH - ANDROID) contient les sources d'une application Android gérant le Bluetooth, permettant de générer un plugin pour Unity3D.

Le dossier [Bluetooth - C#](Bluetooth - C#) contient un client Bluetooth en C# afin de tester la fonctionalité Bluetooth.

Le dossier [Glasses](Glasses) contient les sources de l'application Unity3D prévue pour les lunettes.

Le dossier [PixelSense](PixelSense) contient les sources de l'application pour la table PixelSense.
