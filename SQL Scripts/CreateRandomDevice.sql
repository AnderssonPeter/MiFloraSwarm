DECLARE @macAddress nvarchar(100)
SELECT @macAddress = SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + ':' +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + 
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + ':' +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + 
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + ':' +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + 
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + ':' +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) + ':' +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1) +
                     SUBSTRING(x,(ABS(CHECKSUM(NEWID()))%15)+1,1)
FROM (SELECT x='0123456789ABCDE') a


DECLARE @ipAddress nvarchar(100)
SELECT @ipAddress = CAST(ABS(CHECKSUM(NEWID()))%255 as nvarchar(3)) + '.' +
                    CAST(ABS(CHECKSUM(NEWID()))%255 as nvarchar(3)) + '.' +
                    CAST(ABS(CHECKSUM(NEWID()))%255 as nvarchar(3)) + '.' +
                    CAST(ABS(CHECKSUM(NEWID()))%255 as nvarchar(3))


DECLARE @Names TABLE
(
  Name nvarchar(100),
  Type int
)
INSERT INTO @Names (Name, Type)
VALUES
('Admiring', 1),
('Adoring', 1),
('Affectionate', 1),
('Agitated', 1),
('Amazing', 1),
('Angry', 1),
('Awesome', 1),
('Beautiful', 1),
('Blissful', 1),
('Bold', 1),
('Boring', 1),
('Brave', 1),
('Busy', 1),
('Charming', 1),
('Clever', 1),
('Cool', 1),
('Compassionate', 1),
('Competent', 1),
('Condescending', 1),
('Confident', 1),
('Cranky', 1),
('Crazy', 1),
('Dazzling', 1),
('Determined', 1),
('Distracted', 1),
('Dreamy', 1),
('Eager', 1),
('Ecstatic', 1),
('Elastic', 1),
('Elated', 1),
('Elegant', 1),
('Eloquent', 1),
('Epic', 1),
('Exciting', 1),
('Evil', 1),
('Fervent', 1),
('Festive', 1),
('Flamboyant', 1),
('Sneaky', 1),
('Dark', 1),
('Romantic', 1),
('Bell', 2),
('Black', 2),
('Borg', 2),
('Rabbit', 2),
('Snake', 2),
('Wing', 2),
('Apple', 2),
('Hair', 2),
('Owl', 2),
('Ferret', 2),
('Cat', 2),
('Dog', 2),
('Pear', 2),
('Car', 2),
('Buss', 2),
('Plane', 2),
('Dress', 2),
('Pants', 2),
('Bat', 2),
('Duck', 2),
('Wig', 2),
('Hair', 2),
('Loom', 2),
('Blanket', 2)

DECLARE @name nvarchar(100) = (SELECT TOP 1 name FROM @Names WHERE Type = 1 ORDER BY NEWID()) + (SELECT TOP 1 name FROM @Names WHERE Type = 2 ORDER BY NEWID())
INSERT INTO Devices (MACAddress, IPAddress, Name, Port)
VALUES(@macAddress, @ipAddress, @name, 80)