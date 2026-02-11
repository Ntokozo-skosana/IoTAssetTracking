CREATE DATABASE IoTDevices;
GO

USE IoTDevices;
GO

CREATE TABLE DeviceType (
    DeviceTypeId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,

    Description NVARCHAR(300),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE DeviceGroup (
    GroupId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ParentGroupId INT NULL,

    CONSTRAINT FKey_Group_Parent
         FOREIGN KEY (ParentGroupId) REFERENCES DeviceGroup(GroupId)

);

CREATE TABLE Firmware (
    FirmwareId INT IDENTITY PRIMARY KEY,
    Version NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',

    ReleasedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DeviceTypeId INT NOT NULL,

    CONSTRAINT FKey_Firmware_DeviceType
         FOREIGN KEY (DeviceTypeId) REFERENCES DeviceType(DeviceTypeId)

);

CREATE TABLE Device (
    DeviceId INT IDENTITY PRIMARY KEY,
    SerialNumber NVARCHAR(100) NOT NULL UNIQUE,

    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    DeviceTypeId INT NOT NULL,
    FirmwareId INT NOT NULL,
    GroupId INT NULL,

    CONSTRAINT FKey_Device_DeviceType
         FOREIGN KEY (DeviceTypeId) REFERENCES DeviceType(DeviceTypeId),

    CONSTRAINT FKey_Device_Firmware
         FOREIGN KEY (FirmwareId) REFERENCES Firmware(FirmwareId),

    CONSTRAINT FKey_Device_Group
         FOREIGN KEY (GroupId) REFERENCES DeviceGroup(GroupId)

);
