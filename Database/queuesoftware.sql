
--
-- Table structure for table `store_users`
--

CREATE TABLE `store_users` (
  `storeID` int(5) NOT NULL AUTO_INCREMENT,
  `storename` varchar(255) NOT NULL,
  `address` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  PRIMARY KEY(`storeID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `userID` int(5) NOT NULL AUTO_INCREMENT,
  `username` varchar(255) NOT NULL,
  `firstname` varchar(255) NOT NULL,
  `lastname` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  PRIMARY KEY(`userID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Table structure for table `queues`
--

CREATE TABLE `queues` (
  `queueID` int(5) NOT NULL AUTO_INCREMENT,
  `storename` varchar(255) NOT NULL,
  `storeID` int(5) NOT NULL,
  `address` varchar(255) NOT NULL,
  `product` varchar(255) NOT NULL,
  `date` date NOT NULL,
  PRIMARY KEY(`queueID`),
  FOREIGN KEY(`storeID`) REFERENCES store_users(`storeID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Table structure for table `items`
--


CREATE TABLE `items` (
  `itemID` int(5) NOT NULL AUTO_INCREMENT,
  `firstname` varchar(255) NOT NULL,
  `lastname` varchar(255) NOT NULL,
  `userID` int(5) NOT NULL,
  `product` varchar(255) NOT NULL,
  `queueID` int(5) NOT NULL,
  `storename` varchar(255) NOT NULL,
  `storeID` int(5) NOT NULL,
  `date` date NOT NULL,
  PRIMARY KEY(`itemID`),
  FOREIGN KEY(`userID`) REFERENCES users(`userID`),
  FOREIGN KEY(`queueID`) REFERENCES queues(`queueID`),
  FOREIGN KEY(`storeID`) REFERENCES store_users(`storeID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Table structure for table `products`
--

CREATE TABLE `products` (
  `productID` int(5) NOT NULL AUTO_INCREMENT,
  `productName` varchar(255) NOT NULL,
  `productDescription` varchar(255) NOT NULL,
  `storeName` varchar(255) NOT NULL,
  `dateCreated` varchar(255) NOT NULL,
  PRIMARY KEY(`productID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


--
-- Table structure for table `queuedproducts`
--

CREATE TABLE `queuedproducts` (
  `queuedProductsID` int(5) NOT NULL AUTO_INCREMENT,
  `queueID` int(5) NOT NULL,
  `storeID` int(5) NOT NULL,
  `productID` int(5) NOT NULL,
  `productName` varchar(255) NOT NULL,
  `storeName` varchar(255) NOT NULL,
  `dateCreated` varchar(255) NOT NULL,
  PRIMARY KEY(`queuedProductsID`),
  FOREIGN KEY(`queueID`) REFERENCES queues(`queueID`),
  FOREIGN KEY(`storeID`) REFERENCES store_users(`storeID`),
  FOREIGN KEY(`productID`) REFERENCES products(`productID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


--
-- Table structure for table `queueusers`
--

CREATE TABLE `queueusers` (
  `queueuserID` int(5) NOT NULL AUTO_INCREMENT,
  `queueID` int(5) NOT NULL,
  `storeID` int(5) NOT NULL,
  `productID` int(255) NOT NULL,
  `username` varchar(255) NOT NULL,
  `storeName` varchar(255) NOT NULL,
  `productName` varchar(255) NOT NULL,
  `dateCreated` varchar(255) NOT NULL,
  PRIMARY KEY(`queueuserID`),
  FOREIGN KEY(`queueID`) REFERENCES queues(`queueID`),
  FOREIGN KEY(`storeID`) REFERENCES store_users(`storeID`),
  FOREIGN KEY(`productID`) REFERENCES products(`productID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;