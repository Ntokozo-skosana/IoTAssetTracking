USE IoTDevices;
GO

INSERT INTO DeviceType (Name, Description)
VALUES
('Low-power GPS Tracker', 'Designed for low powered asset tracking with long term asset tracking'),
('Wired GPS Trackers', 'Designed for powered vehicles and equipment, providing real time tracking'),
('Bluetooth Gateways & Sensors', 'Devices that act as Bluetooth hubs to scan nearby Bluetooth tags such as temperature'),
('IoT Data Loggers', 'Robust devices for sensor integration and data logging');

INSERT INTO DeviceGroup (Name, ParentGroupId)
VALUES
('GPS Series', NULL),
('Fusion Series', 1),
('Edge Series', 1),
('Core Series', 1),
('Fusion SubGroup A', 2);

INSERT INTO Firmware (Version, DeviceTypeId)
VALUES
('1.0', 1),
('1.1', 2),
('2.0', 3),
('2.1', 4);

INSERT INTO Device (SerialNumber, DeviceTypeId, FirmwareId, GroupId)
VALUES
('GPS-TRK-2024-0001', 1, 1, 1),
('GPS-WRD-2024-0001', 2, 2, 1),
('GPS-NTK-2025-0002', 3, 3, 2),
('GPS-BAN-2026-0003', 4, 4, 2);
