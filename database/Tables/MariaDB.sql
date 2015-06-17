DROP TABLE IF EXISTS `Folders`;
DROP TABLE IF EXISTS `Documents`;
DROP TABLE IF EXISTS `RemoteServices`;

CREATE TABLE `Folders`
(
	`Id` INT NOT NULL AUTO_INCREMENT,
	`ParentId` INT NULL,
	`Name` VARCHAR(100) NOT NULL,
	`Data` BLOB,
	PRIMARY KEY (`Id`),
	FOREIGN KEY (`ParentId`) REFERENCES `Folders`(`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET utf8
COLLATE utf8_unicode_ci;

CREATE TABLE `Documents`
(
	`Id` INT NOT NULL AUTO_INCREMENT,
	`FolderId` INT NOT NULL,
	`Name` VARCHAR(100) NOT NULL,
	`Data` BLOB,
	PRIMARY KEY (`Id`),
	FOREIGN KEY (`FolderId`) REFERENCES `Folders`(`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET utf8
COLLATE utf8_unicode_ci;

CREATE TABLE `RemoteServices`
(
	`Id` INT NOT NULL AUTO_INCREMENT,
	`Name` VARCHAR(100),
	`ServiceName` VARCHAR(100),
	`Description` TEXT,
	PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET utf8
COLLATE utf8_unicode_ci;

CREATE TABLE `DocumentLinks`
(
	`DocumentId` INT NOT NULL,
	`RemoteServiceId` INT NOT NULL,
	`Uri` VARCHAR(256),
	PRIMARY KEY (`DocumentId`),
	FOREIGN KEY (`DocumentId`) REFERENCES `Documents`(`Id`),
	FOREIGN KEY (`RemoteServiceId`) REFERENCES `RemoteServices`(`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET utf8
COLLATE utf8_unicode_ci;

CREATE INDEX `IX_Folders_ParentId` ON `Folders`(`ParentId`);
CREATE INDEX `IX_Documents_FolderId` ON `Folders`(`FolderId`);
CREATE INDEX `IX_DocumentLinks_DocumentId` ON `DocumentLinks`(`DocumentId`);
CREATE INDEX `IX_DocumentLinks_RemoteServiceId` ON `DocumentLinks`(`RemoteServiceId`);

COMMIT;
