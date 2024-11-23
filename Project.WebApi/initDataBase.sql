/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

CREATE TABLE `PostalAddress` (
                                 `Id` int(11) NOT NULL AUTO_INCREMENT,
                                 `UserId` int(11) NOT NULL,
                                 `Address` varchar(500) NOT NULL,
                                 `PostCode` varchar(50) NOT NULL,
                                 `City` varchar(255) NOT NULL,
                                 `Country` varchar(255) NOT NULL,
                                 PRIMARY KEY (`Id`),
                                 KEY `UserId` (`UserId`),
                                 CONSTRAINT `PostalAddress_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `Role` (
                        `Id` int(11) NOT NULL AUTO_INCREMENT,
                        `Name` varchar(255) NOT NULL,
                        PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `User` (
                        `Id` int(11) NOT NULL AUTO_INCREMENT,
                        `Created` datetime NOT NULL DEFAULT current_timestamp(),
                        `FirstName` varchar(255) NOT NULL,
                        `LastName` varchar(255) NOT NULL,
                        `Email` varchar(255) NOT NULL,
                        `LastConnexion` datetime NOT NULL DEFAULT current_timestamp(),
                        `Password` text NOT NULL,
                        `ProfilePictureUrl` varchar(2083) DEFAULT NULL,
                        `RoleId` int(11) NOT NULL,
                        `IsActivated` tinyint(1) NOT NULL DEFAULT 0,
                        PRIMARY KEY (`Id`),
                        UNIQUE KEY `Email` (`Email`),
                        KEY `RoleId` (`RoleId`),
                        CONSTRAINT `User_ibfk_1` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `UserPreferences` (
                                   `Id` int(11) NOT NULL AUTO_INCREMENT,
                                   `Updated` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
                                   `UserId` int(11) NOT NULL,
                                   `NewsLetter` tinyint(1) NOT NULL,
                                   `EmailNotification` tinyint(1) NOT NULL,
                                   PRIMARY KEY (`Id`),
                                   KEY `UserId` (`UserId`),
                                   CONSTRAINT `UserPreferences_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;



INSERT INTO `Role` (`Id`, `Name`) VALUES
    (1, 'Admin');
INSERT INTO `Role` (`Id`, `Name`) VALUES
    (2, 'User');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;