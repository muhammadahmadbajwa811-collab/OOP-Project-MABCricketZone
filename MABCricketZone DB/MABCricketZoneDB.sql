-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: cricketzonedb
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cart`
--

DROP TABLE IF EXISTS `cart`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cart` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `ProductId` int NOT NULL,
  `Quantity` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `cart_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `cart_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cart`
--

LOCK TABLES `cart` WRITE;
/*!40000 ALTER TABLE `cart` DISABLE KEYS */;
/*!40000 ALTER TABLE `cart` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderitems`
--

DROP TABLE IF EXISTS `orderitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orderitems` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OrderId` int NOT NULL,
  `ProductId` int NOT NULL,
  `Quantity` int NOT NULL DEFAULT '1',
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`Id`),
  KEY `OrderId` (`OrderId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `orderitems_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `orderitems_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderitems`
--

LOCK TABLES `orderitems` WRITE;
/*!40000 ALTER TABLE `orderitems` DISABLE KEYS */;
INSERT INTO `orderitems` VALUES (1,1,15,2,7800.00),(2,1,5,1,2400.00);
/*!40000 ALTER TABLE `orderitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `OrderDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `TotalAmount` decimal(10,2) NOT NULL DEFAULT '0.00',
  `PaymentMethod` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ShippingAddress` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `ReceiptFilePath` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,2,'2026-06-11 01:46:01',18000.00,'Debit Card','Rehman Colony','Confirmed','/receipts/Receipt_0_20260611014601.txt');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `Brand` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `Description` varchar(1000) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Quantity` int NOT NULL DEFAULT '0',
  `ImageUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'English Willow Cricket Bat Grade 1','SS','Premium Grade 1 English Willow bat, hand-crafted with 6 grains. Full toe protection, premium leather grip. Perfect for professionals.',14500.00,30,'https://images.unsplash.com/photo-1531415074968-036ba1b575da?w=600&q=80'),(2,'Kashmir Willow Power Bat','MRF','Full size Kashmir Willow bat with thick edges and prominent spine. Ideal for hard pitches and big hitting. Comes with grip and cover.',6800.00,50,'https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=600&q=80'),(3,'Genius Grand Edition Bat','SG','The legendary SG Genius Grand Edition used by top players. Round spine profile, balanced pickup, suitable for all styles.',18900.00,15,'https://images.unsplash.com/photo-1531415074968-036ba1b575da?w=600&q=80'),(4,'Kookaburra Kahuna Pro Bat','Kookaburra','Australian craftsmanship with high-performance willow. Low sweet spot for ground shots, ideal for all pitches.',22000.00,10,'https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=600&q=80'),(5,'Dukes County Championship Ball','Dukes','Hand-stitched red leather ball 5.5oz. Premium cork core for consistent swing and seam movement. Used in county cricket.',2400.00,99,'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=600&q=80'),(6,'SG Test Match Ball','SG','Official match ball. Premium leather, tight seam, excellent durability. Available in red and white.',3200.00,80,'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=600&q=80'),(7,'Tape Ball Cricket Ball (Pack of 12)','Generic','Standard tape ball for gully cricket and indoor practice. Soft core, durable tape covering. Suitable for all ages.',850.00,200,'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=600&q=80'),(8,'Pro Batting Gloves - Right Hand','SS','Premium leather batting gloves with reinforced fingers, moisture-wicking inner, and flexible wrist strap.',4200.00,40,'https://images.unsplash.com/photo-1599586120429-48281b6f0ece?w=600&q=80'),(9,'Wicket Keeping Gloves','Kookaburra','Professional WK gloves with thick inner padding, webbed fingers, reinforced palm. Suitable for first-class cricket.',5500.00,25,'https://images.unsplash.com/photo-1599586120429-48281b6f0ece?w=600&q=80'),(10,'Cricket Batting Pads - Senior','MRF','Lightweight PVC outer with thick foam inner. 3 straps for secure fit. Knee roll protection. Available in white.',3800.00,35,'https://images.unsplash.com/photo-1531415074968-036ba1b575da?w=600&q=80'),(11,'ABS Cricket Helmet with Steel Grille','SG','Certified safety helmet with ABS outer shell, foam inner lining, adjustable chin strap, removable steel grille.',5200.00,20,'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=600&q=80'),(13,'Cricket Abdominal Guard','SS','High-density foam with hard plastic outer shell. Essential protective gear for batting and wicket-keeping.',1200.00,60,'https://images.unsplash.com/photo-1599586120429-48281b6f0ece?w=600&q=80'),(14,'Cricket Whites - Full Set (Shirt + Trousers)','Nike','Breathable polyester blend cricket whites. Anti-sweat technology, slim fit, reinforced knees. S/M/L/XL.',3500.00,45,'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=600&q=80'),(15,'Cricket Shoes - Rubber Sole','Adidas','Lightweight cricket shoes with rubber studs for grip on all surfaces. Breathable mesh upper, cushioned sole.',7800.00,28,'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600&q=80'),(16,'Batting Thigh Pad','SG','External thigh pad with full coverage. Lightweight, shock-absorbing. Velcro strap for easy attachment.',900.00,70,'https://images.unsplash.com/photo-1599586120429-48281b6f0ece?w=600&q=80');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PasswordHash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Customer',
  `FullName` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `Address` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','admin123','Admin','MAB Admin','123 Admin Street, Lahore','2026-06-11 01:40:30'),(2,'customer1','pass123','Customer','Ali Bajwa','456 Model Town, Lahore','2026-06-11 01:40:30'),(3,'customer2','pass123','Customer','Usman Ahmed','789 DHA Phase 5, Lahore','2026-06-11 01:40:30'),(4,'customer3','pass123','Customer','Sara Khan','12 Gulberg III, Lahore','2026-06-11 01:40:30');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-25 10:05:16
