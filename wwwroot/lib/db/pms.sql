-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 04, 2026 at 02:11 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `pms`
--

-- --------------------------------------------------------

--
-- Table structure for table `customer`
--

CREATE TABLE `customer` (
  `CustomerID` int(11) NOT NULL,
  `CustomerCode` varchar(50) NOT NULL,
  `CustomerName` varchar(200) NOT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `customer`
--

INSERT INTO `customer` (`CustomerID`, `CustomerCode`, `CustomerName`, `Remark`, `Status`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 'CUS-001', 'Meridian Technologies Sdn Bhd', 'Enterprise software and infrastructure services', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(2, 'CUS-002', 'Northstar Retail Group', 'Retail operations and point-of-sale systems', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(3, 'CUS-003', 'Greenfield Manufacturing', 'Manufacturing and production automation', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(4, 'CUS-004', 'Apex Healthcare Solutions', 'Healthcare information systems', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(5, 'CUS-005', 'Bluewave Logistics', 'Warehouse and logistics management', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(6, 'CUS-006', 'Crescent Education Group', 'Education and campus management', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(7, 'CUS-007', 'UrbanNest Properties', 'Property and facilities management', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(8, 'CUS-008', 'Silverline Finance', 'Financial services and reporting', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(9, 'CUS-009', 'BrightPath Media', 'Digital media and content platforms', 1, '2026-11-03 10:00:00', 1, NULL, NULL),
(10, 'CUS-010', 'Everpeak Energy', 'Energy monitoring and maintenance', 1, '2026-11-03 10:00:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `location`
--

CREATE TABLE `location` (
  `LocationID` int(11) NOT NULL,
  `LocationCode` varchar(50) NOT NULL,
  `LocationName` varchar(200) NOT NULL,
  `FullAddress` varchar(500) DEFAULT NULL,
  `PICID` int(11) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `location`
--

INSERT INTO `location` (`LocationID`, `LocationCode`, `LocationName`, `FullAddress`, `PICID`, `Remark`, `Status`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 'LOC-001', 'Meridian HQ', 'Level 12, Menara Meridian, Kuala Lumpur', 1, 'Head office', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(2, 'LOC-002', 'Northstar Mid Valley', 'Lot G-025, Mid Valley Megamall, Kuala Lumpur', 2, 'Retail pilot site', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(3, 'LOC-003', 'Greenfield Plant A', 'Lot 18, Kawasan Perindustrian Shah Alam, Selangor', 3, 'Main production plant', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(4, 'LOC-004', 'Apex Medical Centre', 'Jalan Tun Razak, Kuala Lumpur', 4, 'Main clinic', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(5, 'LOC-005', 'Bluewave Distribution Hub', 'Jalan Pelabuhan, Klang, Selangor', 5, 'Distribution centre', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(6, 'LOC-006', 'Crescent Main Campus', 'Jalan Universiti, Petaling Jaya, Selangor', 6, 'Main campus', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(7, 'LOC-007', 'UrbanNest Tower', 'Persiaran Tropicana, Petaling Jaya, Selangor', 7, 'Property management office', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(8, 'LOC-008', 'Silverline Office', 'Jalan Sultan Ismail, Kuala Lumpur', 8, 'Finance department', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(9, 'LOC-009', 'BrightPath Studio', 'Jalan Bangsar, Kuala Lumpur', 9, 'Creative studio', 1, '2026-11-03 10:25:00', 1, NULL, NULL),
(10, 'LOC-010', 'Everpeak Operations Centre', 'Cyberjaya, Selangor', 10, 'Operations centre', 1, '2026-11-03 10:25:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `payment`
--

CREATE TABLE `payment` (
  `PaymentID` int(11) NOT NULL,
  `ProjectID` int(11) NOT NULL,
  `PaymentReference` varchar(100) DEFAULT NULL,
  `PaymentDescription` varchar(500) DEFAULT NULL,
  `Amount` decimal(15,2) NOT NULL,
  `PaymentDate` date DEFAULT NULL,
  `DueDate` date DEFAULT NULL,
  `PaymentMethod` varchar(50) DEFAULT NULL,
  `PaymentStatus` varchar(30) NOT NULL DEFAULT 'Pending',
  `Remark` varchar(500) DEFAULT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `payment`
--

INSERT INTO `payment` (`PaymentID`, `ProjectID`, `PaymentReference`, `PaymentDescription`, `Amount`, `PaymentDate`, `DueDate`, `PaymentMethod`, `PaymentStatus`, `Remark`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 1, 'PAY-2026-001', 'Initial project deposit', 37000.00, '2026-01-20', '2026-01-20', 'Bank Transfer', 'Paid', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(2, 2, 'PAY-2026-002', 'Retail POS discovery milestone', 18400.00, '2026-03-01', '2026-03-15', 'Bank Transfer', 'Paid', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(3, 3, 'PAY-2026-003', 'IoT equipment procurement', 62000.00, '2026-04-15', '2026-04-30', 'Purchase Order', 'Pending', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(4, 4, 'PAY-2026-004', 'Final clinic portal milestone', 27300.00, '2026-08-20', '2026-08-31', 'Bank Transfer', 'Paid', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(5, 5, 'PAY-2026-005', 'Warehouse system development milestone', 29250.00, '2026-07-01', '2026-07-15', 'Bank Transfer', 'Overdue', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(6, 6, 'PAY-2026-006', 'Student system requirements milestone', 31800.00, '2026-06-15', '2026-06-30', 'Bank Transfer', 'Paid', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(7, 7, 'PAY-2026-007', 'Property app final payment', 15700.00, '2026-08-01', '2026-08-15', 'Bank Transfer', 'Paid', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(8, 8, 'PAY-2026-008', 'Reporting automation first milestone', 26000.00, '2026-07-01', '2026-07-15', 'Bank Transfer', 'Pending', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(9, 9, 'PAY-2026-009', 'Media library project deposit', 13300.00, '2026-07-10', '2026-07-25', 'Bank Transfer', 'Pending', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL),
(10, 10, 'PAY-2026-010', 'Energy dashboard design milestone', 32000.00, '2026-04-01', '2026-04-15', 'Bank Transfer', 'Paid', 'Sample payment record', '2026-11-03 10:35:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `personincharge`
--

CREATE TABLE `personincharge` (
  `PICID` int(11) NOT NULL,
  `PICCode` varchar(50) NOT NULL,
  `PICName` varchar(150) NOT NULL,
  `PICRole` varchar(100) DEFAULT NULL,
  `CustomerID` int(11) NOT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `personincharge`
--

INSERT INTO `personincharge` (`PICID`, `PICCode`, `PICName`, `PICRole`, `CustomerID`, `Remark`, `Status`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 'PIC-001', 'Marcus Goh', 'IT Director', 1, 'Primary customer contact 1', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(2, 'PIC-002', 'Elaine Chua', 'Project Coordinator', 2, 'Primary customer contact 2', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(3, 'PIC-003', 'Ravi Kumar', 'Operations Manager', 3, 'Primary customer contact 3', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(4, 'PIC-004', 'Dr. Hannah Lee', 'IT Manager', 4, 'Primary customer contact 4', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(5, 'PIC-005', 'Nurul Iman', 'Logistics Manager', 5, 'Primary customer contact 5', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(6, 'PIC-006', 'Steven Tan', 'Campus Administrator', 6, 'Primary customer contact 6', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(7, 'PIC-007', 'Cheryl Wong', 'Facilities Director', 7, 'Primary customer contact 7', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(8, 'PIC-008', 'Amirul Azman', 'Finance Manager', 8, 'Primary customer contact 8', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(9, 'PIC-009', 'Michelle Yap', 'Marketing Director', 9, 'Primary customer contact 9', 1, '2026-11-03 10:15:00', 1, NULL, NULL),
(10, 'PIC-010', 'Kumaravel Raj', 'Engineering Manager', 10, 'Primary customer contact 10', 1, '2026-11-03 10:15:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `project`
--

CREATE TABLE `project` (
  `ProjectID` int(11) NOT NULL,
  `ProjectCode` varchar(50) NOT NULL,
  `ProjectName` varchar(200) NOT NULL,
  `CustomerID` int(11) NOT NULL,
  `Price` decimal(15,2) NOT NULL DEFAULT 0.00,
  `StartDate` date DEFAULT NULL,
  `DueDate` date DEFAULT NULL,
  `Description` text DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `Status` varchar(30) NOT NULL DEFAULT 'Pending',
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `project`
--

INSERT INTO `project` (`ProjectID`, `ProjectCode`, `ProjectName`, `CustomerID`, `Price`, `StartDate`, `DueDate`, `Description`, `Remark`, `Status`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 'PRJ-2026-001', 'Enterprise Resource Planning Upgrade', 1, 185000.00, '2026-01-15', '2026-12-15', 'Upgrade core ERP modules, integrations and reporting.', 'Phase 2 implementation', 'In Progress', '2026-11-03 10:20:00', 1, NULL, NULL),
(2, 'PRJ-2026-002', 'Retail POS Modernisation', 2, 92000.00, '2026-02-01', '2026-09-30', 'Modernise point-of-sale terminals and inventory sync.', 'Pilot completed', 'In Progress', '2026-11-03 10:20:00', 1, NULL, NULL),
(3, 'PRJ-2026-003', 'Factory IoT Monitoring', 3, 248000.00, '2026-03-01', '2027-02-28', 'Deploy IoT sensors and production monitoring dashboards.', 'Hardware procurement', 'Pending', '2026-11-03 10:20:00', 1, NULL, NULL),
(4, 'PRJ-2026-004', 'Clinic Management Portal', 4, 136500.00, '2026-01-10', '2026-08-31', 'Patient appointment and clinic administration portal.', 'UAT scheduled', 'Completed', '2026-11-03 10:20:00', 1, NULL, NULL),
(5, 'PRJ-2026-005', 'Warehouse Tracking System', 5, 117000.00, '2026-04-01', '2026-11-30', 'Barcode-based warehouse tracking and dispatch system.', 'Integration testing', 'In Progress', '2026-11-03 10:20:00', 1, NULL, NULL),
(6, 'PRJ-2026-006', 'Student Information System', 6, 159000.00, '2026-05-01', '2027-01-31', 'Student enrolment, timetable and academic records platform.', 'Requirements approved', 'In Progress', '2026-11-03 10:20:00', 1, NULL, NULL),
(7, 'PRJ-2026-007', 'Property Maintenance App', 7, 78500.00, '2026-02-15', '2026-07-31', 'Mobile maintenance requests and contractor assignment.', 'Project delivered', 'Completed', '2026-11-03 10:20:00', 1, NULL, NULL),
(8, 'PRJ-2026-008', 'Financial Reporting Automation', 8, 104000.00, '2026-06-01', '2026-12-31', 'Automate monthly reporting and reconciliation workflows.', 'Development started', 'In Progress', '2026-11-03 10:20:00', 1, NULL, NULL),
(9, 'PRJ-2026-009', 'Media Asset Library', 9, 66500.00, '2026-07-01', '2027-01-15', 'Centralised media asset library with approval workflow.', 'Pending kickoff', 'Pending', '2026-11-03 10:20:00', 1, NULL, NULL),
(10, 'PRJ-2026-010', 'Energy Usage Dashboard', 10, 128000.00, '2026-03-15', '2026-10-15', 'Monitor energy consumption and equipment efficiency.', 'Final review', 'On Hold', '2026-11-03 10:20:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `projectmember`
--

CREATE TABLE `projectmember` (
  `ProjectMemberID` int(11) NOT NULL,
  `ProjectID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `MemberRole` varchar(50) NOT NULL DEFAULT 'Member',
  `AssignedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `Status` tinyint(1) NOT NULL DEFAULT 1,
  `Remark` varchar(500) DEFAULT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `projectmember`
--

INSERT INTO `projectmember` (`ProjectMemberID`, `ProjectID`, `UserID`, `MemberRole`, `AssignedDate`, `Status`, `Remark`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 1, 2, 'Project Manager', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(2, 2, 3, 'Lead Developer', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(3, 3, 4, 'Developer', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(4, 4, 5, 'Finance Representative', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(5, 5, 6, 'Project Coordinator', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(6, 6, 7, 'Operations Lead', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(7, 7, 8, 'Sponsor', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(8, 8, 9, 'Auditor', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(9, 9, 10, 'Document Controller', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL),
(10, 10, 11, 'Project Member', '2026-11-03 10:30:00', 1, 'Sample project team assignment', '2026-11-03 10:30:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `UserID` int(11) NOT NULL,
  `UserName` varchar(100) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `UserGroupID` int(11) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL,
  `EditedDate` datetime DEFAULT NULL,
  `EditedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`UserID`, `UserName`, `Password`, `Email`, `UserGroupID`, `Remark`, `Status`, `CreatedDate`, `CreatedBy`, `EditedDate`, `EditedBy`) VALUES
(1, 'Choo Yin Keat', 'AQAAAAIAAYagAAAAEAYBhY1uerzQZ4hfu0ZVuxjax4R8O3DUAxRyJmT9oLC3Aw29JXenirJsugMeXzdCaw==', 'keatyinchoo@gmail.com', 1, NULL, 1, '2026-10-18 13:35:19', NULL, NULL, NULL),
(2, 'Alicia Tan', 'AQAAAAIAAYagAAAAEDzjXtihR2unrHM+hhxhcdokg3n+Xk0yDQe72MAQyKX6oEhfzggHsDFIokoaF+hxmg==', 'alicia.tan@pmsdemo.com', 2, 'Project manager', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(3, 'Daniel Lim', 'AQAAAAIAAYagAAAAEHv3XGez2N2DCW65Lns4sKbZIL/LSWlhG7jLLNzSPEW9MjAiVH39+0p+3WzzD6wyag==', 'daniel.lim@pmsdemo.com', 3, 'Senior software developer', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(4, 'Nur Aisyah', 'AQAAAAIAAYagAAAAEPTcu/2X81qrVWcyWTZ3gyVGxm90zaJp/UJGohjW2nBMbV2IO0IhvCjrnKOYO4iZRA==', 'aisyah.nur@pmsdemo.com', 3, 'Software developer', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(5, 'Jason Wong', 'AQAAAAIAAYagAAAAEB6jMzwH7qwkWJCyqKT5A+5zEjeL1N/pg3A9JUdoA/5FG8dljYdShtOI73D1h4AVZg==', 'jason.wong@pmsdemo.com', 4, 'Finance executive', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(6, 'Priya Nair', 'AQAAAAIAAYagAAAAELlpw65Hpkg9kSjOBuhu2LDhrSQnmGnoJ+1b6Afit76fdjo7usANlXgRRF+4/Ug0gQ==', 'priya.nair@pmsdemo.com', 5, 'Customer relationship executive', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(7, 'Muhammad Farid', 'AQAAAAIAAYagAAAAEPQSKBsn4lFVAMGiUwixPr8ntwJqYdVwfymW6TAaCAATOEUEPI6a/OOphoLi4ag82A==', 'farid.m@pmsdemo.com', 6, 'Operations coordinator', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(8, 'Samantha Lee', 'AQAAAAIAAYagAAAAEGee+N7UR1fH8RkzG0xR9C7+VZc1w/kIRVeQhKxn62Vs+6ZjyiZF6y6XoZLN+iBHOw==', 'samantha.lee@pmsdemo.com', 7, 'Management user', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(9, 'Kevin Chong', 'AQAAAAIAAYagAAAAEC5Op58787ZuiEl8zGtCNhE+NoKYwYs50V49h8vs735+IX2qxx1y8GJw785xHE8XfQ==', 'kevin.chong@pmsdemo.com', 8, 'Internal auditor', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(10, 'Mei Ling', 'AQAAAAIAAYagAAAAEPcN1EOgd1rz0sg4FYQkWHemz4CsFBhWHgEboacEmt7X/G6N7ymPtGgAsCtVbKbrng==', 'mei.ling@pmsdemo.com', 9, 'Document controller', 1, '2026-11-03 10:10:00', 1, NULL, NULL),
(11, 'Arif Hakim', 'AQAAAAIAAYagAAAAEGKj+ltim0Tqg0rPwbHfSqB+Q62QLHejavz3qYtaaZq0EdDmqrB94I0h+P1yn8MgfA==', 'arif.hakim@pmsdemo.com', 10, 'Read-only project user', 1, '2026-11-03 10:10:00', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `usergroup`
--

CREATE TABLE `usergroup` (
  `UserGroupID` int(11) NOT NULL,
  `UserGroupName` varchar(100) NOT NULL,
  `UserGroupDescription` varchar(255) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `EditedDate` datetime DEFAULT NULL,
  `CreatedBy` varchar(100) DEFAULT NULL,
  `EditedBy` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usergroup`
--

INSERT INTO `usergroup` (`UserGroupID`, `UserGroupName`, `UserGroupDescription`, `Status`, `CreatedDate`, `EditedDate`, `CreatedBy`, `EditedBy`) VALUES
(1, 'Administrator', 'System Administrators with full access to all modules.', 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(2, 'Project Manager', 'Manage assigned projects, members, schedules and project reports.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(3, 'Developer', 'Access project details and update technical progress.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(4, 'Finance', 'Manage payment records and financial reports.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(5, 'Customer Service', 'Maintain customer and contact information.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(6, 'Operations', 'Manage operational locations and project assignments.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(7, 'Management', 'View business-wide project and financial information.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(8, 'Auditor', 'Read-only access with report printing.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(9, 'Document Controller', 'Manage document-related module permissions.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL),
(10, 'Read Only', 'View permitted modules without modifying records.', 1, '2026-11-03 10:05:00', NULL, 'System', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `usergroupaccess`
--

CREATE TABLE `usergroupaccess` (
  `UGAID` int(11) NOT NULL,
  `UGAGroupID` int(11) NOT NULL,
  `UGAModule` varchar(100) NOT NULL,
  `UGAView` tinyint(1) NOT NULL DEFAULT 0,
  `UGAAdd` tinyint(1) NOT NULL DEFAULT 0,
  `UGAEdit` tinyint(1) NOT NULL DEFAULT 0,
  `UGADelete` tinyint(1) NOT NULL DEFAULT 0,
  `UGAPrint` tinyint(1) NOT NULL DEFAULT 0,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp(),
  `EditedDate` datetime DEFAULT NULL,
  `CreatedBy` varchar(100) DEFAULT NULL,
  `EditedBy` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usergroupaccess`
--

INSERT INTO `usergroupaccess` (`UGAID`, `UGAGroupID`, `UGAModule`, `UGAView`, `UGAAdd`, `UGAEdit`, `UGADelete`, `UGAPrint`, `CreatedDate`, `EditedDate`, `CreatedBy`, `EditedBy`) VALUES
(1, 1, 'User', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(2, 1, 'UserGroup', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(3, 1, 'Customer', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(4, 1, 'PersonInCharge', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(5, 1, 'Location', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(6, 1, 'Project', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(7, 1, 'Payment', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(8, 1, 'Document', 1, 1, 1, 1, 1, '2026-11-03 21:48:24', NULL, 'System', NULL),
(9, 2, 'User', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(10, 2, 'UserGroup', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(11, 2, 'Customer', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(12, 2, 'PersonInCharge', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(13, 2, 'Location', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(14, 2, 'Project', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(15, 2, 'Payment', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(16, 2, 'Document', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(17, 3, 'User', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(18, 3, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(19, 3, 'Customer', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(20, 3, 'PersonInCharge', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(21, 3, 'Location', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(22, 3, 'Project', 1, 0, 1, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(23, 3, 'Payment', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(24, 3, 'Document', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(25, 4, 'User', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(26, 4, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(27, 4, 'Customer', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(28, 4, 'PersonInCharge', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(29, 4, 'Location', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(30, 4, 'Project', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(31, 4, 'Payment', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(32, 4, 'Document', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(33, 5, 'User', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(34, 5, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(35, 5, 'Customer', 1, 1, 1, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(36, 5, 'PersonInCharge', 1, 1, 1, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(37, 5, 'Location', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(38, 5, 'Project', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(39, 5, 'Payment', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(40, 5, 'Document', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(41, 6, 'User', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(42, 6, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(43, 6, 'Customer', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(44, 6, 'PersonInCharge', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(45, 6, 'Location', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(46, 6, 'Project', 1, 0, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(47, 6, 'Payment', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(48, 6, 'Document', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(49, 7, 'User', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(50, 7, 'UserGroup', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(51, 7, 'Customer', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(52, 7, 'PersonInCharge', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(53, 7, 'Location', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(54, 7, 'Project', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(55, 7, 'Payment', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(56, 7, 'Document', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(57, 8, 'User', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(58, 8, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(59, 8, 'Customer', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(60, 8, 'PersonInCharge', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(61, 8, 'Location', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(62, 8, 'Project', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(63, 8, 'Payment', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(64, 8, 'Document', 1, 0, 0, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(65, 9, 'User', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(66, 9, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(67, 9, 'Customer', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(68, 9, 'PersonInCharge', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(69, 9, 'Location', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(70, 9, 'Project', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(71, 9, 'Payment', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(72, 9, 'Document', 1, 1, 1, 0, 1, '2026-11-03 10:40:00', NULL, 'System', NULL),
(73, 10, 'User', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(74, 10, 'UserGroup', 0, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(75, 10, 'Customer', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(76, 10, 'PersonInCharge', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(77, 10, 'Location', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(78, 10, 'Project', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(79, 10, 'Payment', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL),
(80, 10, 'Document', 1, 0, 0, 0, 0, '2026-11-03 10:40:00', NULL, 'System', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `customer`
--
ALTER TABLE `customer`
  ADD PRIMARY KEY (`CustomerID`),
  ADD UNIQUE KEY `UK_Customer_Code` (`CustomerCode`),
  ADD KEY `FK_Customer_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_Customer_EditedBy` (`EditedBy`);

--
-- Indexes for table `location`
--
ALTER TABLE `location`
  ADD PRIMARY KEY (`LocationID`),
  ADD UNIQUE KEY `UK_Location_Code` (`LocationCode`),
  ADD KEY `IX_Location_PICID` (`PICID`),
  ADD KEY `FK_Location_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_Location_EditedBy` (`EditedBy`);

--
-- Indexes for table `payment`
--
ALTER TABLE `payment`
  ADD PRIMARY KEY (`PaymentID`),
  ADD UNIQUE KEY `UK_Payment_Reference` (`PaymentReference`),
  ADD KEY `IX_Payment_ProjectID` (`ProjectID`),
  ADD KEY `IX_Payment_Status` (`PaymentStatus`),
  ADD KEY `FK_Payment_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_Payment_EditedBy` (`EditedBy`);

--
-- Indexes for table `personincharge`
--
ALTER TABLE `personincharge`
  ADD PRIMARY KEY (`PICID`),
  ADD UNIQUE KEY `UK_PIC_Code` (`PICCode`),
  ADD KEY `IX_PIC_CustomerID` (`CustomerID`),
  ADD KEY `FK_PIC_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_PIC_EditedBy` (`EditedBy`);

--
-- Indexes for table `project`
--
ALTER TABLE `project`
  ADD PRIMARY KEY (`ProjectID`),
  ADD UNIQUE KEY `UK_Project_Code` (`ProjectCode`),
  ADD KEY `IX_Project_CustomerID` (`CustomerID`),
  ADD KEY `IX_Project_Status` (`Status`),
  ADD KEY `FK_Project_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_Project_EditedBy` (`EditedBy`);

--
-- Indexes for table `projectmember`
--
ALTER TABLE `projectmember`
  ADD PRIMARY KEY (`ProjectMemberID`),
  ADD UNIQUE KEY `UK_ProjectMember_Project_User` (`ProjectID`,`UserID`),
  ADD KEY `IX_ProjectMember_UserID` (`UserID`),
  ADD KEY `FK_ProjectMember_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_ProjectMember_EditedBy` (`EditedBy`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `UK_User_UserName` (`UserName`),
  ADD UNIQUE KEY `UK_User_Email` (`Email`),
  ADD KEY `FK_User_CreatedBy` (`CreatedBy`),
  ADD KEY `FK_User_EditedBy` (`EditedBy`);

--
-- Indexes for table `usergroup`
--
ALTER TABLE `usergroup`
  ADD PRIMARY KEY (`UserGroupID`);

--
-- Indexes for table `usergroupaccess`
--
ALTER TABLE `usergroupaccess`
  ADD PRIMARY KEY (`UGAID`),
  ADD UNIQUE KEY `UX_UGA_Group_Module` (`UGAGroupID`,`UGAModule`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `customer`
--
ALTER TABLE `customer`
  MODIFY `CustomerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `location`
--
ALTER TABLE `location`
  MODIFY `LocationID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `payment`
--
ALTER TABLE `payment`
  MODIFY `PaymentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `personincharge`
--
ALTER TABLE `personincharge`
  MODIFY `PICID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `ProjectID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `projectmember`
--
ALTER TABLE `projectmember`
  MODIFY `ProjectMemberID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `usergroup`
--
ALTER TABLE `usergroup`
  MODIFY `UserGroupID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `usergroupaccess`
--
ALTER TABLE `usergroupaccess`
  MODIFY `UGAID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=81;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `customer`
--
ALTER TABLE `customer`
  ADD CONSTRAINT `FK_Customer_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_Customer_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL;

--
-- Constraints for table `location`
--
ALTER TABLE `location`
  ADD CONSTRAINT `FK_Location_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_Location_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_Location_PIC` FOREIGN KEY (`PICID`) REFERENCES `personincharge` (`PICID`) ON DELETE SET NULL;

--
-- Constraints for table `payment`
--
ALTER TABLE `payment`
  ADD CONSTRAINT `FK_Payment_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_Payment_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_Payment_Project` FOREIGN KEY (`ProjectID`) REFERENCES `project` (`ProjectID`);

--
-- Constraints for table `personincharge`
--
ALTER TABLE `personincharge`
  ADD CONSTRAINT `FK_PIC_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_PIC_Customer` FOREIGN KEY (`CustomerID`) REFERENCES `customer` (`CustomerID`),
  ADD CONSTRAINT `FK_PIC_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL;

--
-- Constraints for table `project`
--
ALTER TABLE `project`
  ADD CONSTRAINT `FK_Project_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_Project_Customer` FOREIGN KEY (`CustomerID`) REFERENCES `customer` (`CustomerID`),
  ADD CONSTRAINT `FK_Project_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL;

--
-- Constraints for table `projectmember`
--
ALTER TABLE `projectmember`
  ADD CONSTRAINT `FK_ProjectMember_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_ProjectMember_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_ProjectMember_Project` FOREIGN KEY (`ProjectID`) REFERENCES `project` (`ProjectID`),
  ADD CONSTRAINT `FK_ProjectMember_User` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`);

--
-- Constraints for table `user`
--
ALTER TABLE `user`
  ADD CONSTRAINT `FK_User_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_User_EditedBy` FOREIGN KEY (`EditedBy`) REFERENCES `user` (`UserID`) ON DELETE SET NULL;

--
-- Constraints for table `usergroupaccess`
--
ALTER TABLE `usergroupaccess`
  ADD CONSTRAINT `FK_UGA_UserGroup` FOREIGN KEY (`UGAGroupID`) REFERENCES `usergroup` (`UserGroupID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
