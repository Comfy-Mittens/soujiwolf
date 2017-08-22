










































-- -----------------------------------------------------------
-- Entity Designer DDL Script for MySQL Server 4.1 and higher
-- -----------------------------------------------------------
-- Date Created: 08/21/2017 16:16:55

-- Generated from EDMX file: C:\Users\Arthur\Documents\Visual Studio 2017\Projects\Sharlayan\Soujiwolf\Entities\Soujiwolf.edmx
-- Target version: 3.0.0.0

-- --------------------------------------------------



-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- NOTE: if the constraint does not exist, an ignorable error will be reported.
-- --------------------------------------------------


--    ALTER TABLE `PlayerRoles` DROP CONSTRAINT `FK_PlayerRoleGameRole`;

--    ALTER TABLE `BroadcastListeners` DROP CONSTRAINT `FK_BroadcastListenerBroadcast`;


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------
SET foreign_key_checks = 0;

    DROP TABLE IF EXISTS `ModeratorRoles`;

    DROP TABLE IF EXISTS `Votes`;

    DROP TABLE IF EXISTS `WerewolfGame`;

    DROP TABLE IF EXISTS `GameRoles`;

    DROP TABLE IF EXISTS `PlayerRoles`;

    DROP TABLE IF EXISTS `Broadcasts`;

    DROP TABLE IF EXISTS `BroadcastListeners`;

SET foreign_key_checks = 1;

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------


CREATE TABLE `ModeratorRoles`(
	`GuildSnowflake` bigint NOT NULL, 
	`RoleSnowflake` bigint NOT NULL);

ALTER TABLE `ModeratorRoles` ADD PRIMARY KEY (`GuildSnowflake`, `RoleSnowflake`);





CREATE TABLE `Votes`(
	`GuildSnowflake` bigint NOT NULL, 
	`Voter` bigint NOT NULL, 
	`Voted` bigint NOT NULL, 
	`Changed` bool NOT NULL);

ALTER TABLE `Votes` ADD PRIMARY KEY (`GuildSnowflake`, `Voter`);





CREATE TABLE `WerewolfGame`(
	`GuildSnowflake` bigint NOT NULL, 
	`PlayerRoleSnowflake` bigint NOT NULL, 
	`NarratorRoleSnowflake` bigint NOT NULL, 
	`VillageChannelSnowflake` bigint NOT NULL, 
	`WerewolfChannelSnowflake` bigint NOT NULL, 
	`Status` longtext NOT NULL, 
	`DeadRoleSnowflake` bigint NOT NULL);

ALTER TABLE `WerewolfGame` ADD PRIMARY KEY (`GuildSnowflake`);





CREATE TABLE `GameRoles`(
	`GuildSnowflake` bigint NOT NULL, 
	`Name` varchar (30) NOT NULL, 
	`Color` bigint NOT NULL, 
	`ThumbnailImage` longtext NOT NULL, 
	`Description` longtext NOT NULL, 
	`Active` bool NOT NULL, 
	`Rate` double, 
	`Affinity` int NOT NULL);

ALTER TABLE `GameRoles` ADD PRIMARY KEY (`GuildSnowflake`, `Name`);





CREATE TABLE `PlayerRoles`(
	`PlayerSnowflake` bigint NOT NULL, 
	`GameRole_GuildSnowflake` bigint NOT NULL, 
	`GameRole_Name` varchar (30) NOT NULL);

ALTER TABLE `PlayerRoles` ADD PRIMARY KEY (`PlayerSnowflake`);





CREATE TABLE `Broadcasts`(
	`BroadcastId` CHAR(36) BINARY NOT NULL, 
	`GuildSnowflake` bigint NOT NULL, 
	`ChannelSnowflake` bigint NOT NULL);

ALTER TABLE `Broadcasts` ADD PRIMARY KEY (`BroadcastId`, `GuildSnowflake`, `ChannelSnowflake`);





CREATE TABLE `BroadcastListeners`(
	`GuildSnowflake` bigint NOT NULL, 
	`ChannelSnowflake` bigint NOT NULL, 
	`BroadcastBroadcastId` CHAR(36) BINARY NOT NULL, 
	`BroadcastGuildSnowflake` bigint NOT NULL, 
	`BroadcastChannelSnowflake` bigint NOT NULL);

ALTER TABLE `BroadcastListeners` ADD PRIMARY KEY (`GuildSnowflake`, `ChannelSnowflake`, `BroadcastBroadcastId`, `BroadcastGuildSnowflake`, `BroadcastChannelSnowflake`);







-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------


-- Creating foreign key on `GameRole_GuildSnowflake`, `GameRole_Name` in table 'PlayerRoles'

ALTER TABLE `PlayerRoles`
ADD CONSTRAINT `FK_PlayerRoleGameRole`
    FOREIGN KEY (`GameRole_GuildSnowflake`, `GameRole_Name`)
    REFERENCES `GameRoles`
        (`GuildSnowflake`, `Name`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating non-clustered index for FOREIGN KEY 'FK_PlayerRoleGameRole'

CREATE INDEX `IX_FK_PlayerRoleGameRole`
    ON `PlayerRoles`
    (`GameRole_GuildSnowflake`, `GameRole_Name`);



-- Creating foreign key on `BroadcastBroadcastId`, `BroadcastGuildSnowflake`, `BroadcastChannelSnowflake` in table 'BroadcastListeners'

ALTER TABLE `BroadcastListeners`
ADD CONSTRAINT `FK_BroadcastListenerBroadcast`
    FOREIGN KEY (`BroadcastBroadcastId`, `BroadcastGuildSnowflake`, `BroadcastChannelSnowflake`)
    REFERENCES `Broadcasts`
        (`BroadcastId`, `GuildSnowflake`, `ChannelSnowflake`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating non-clustered index for FOREIGN KEY 'FK_BroadcastListenerBroadcast'

CREATE INDEX `IX_FK_BroadcastListenerBroadcast`
    ON `BroadcastListeners`
    (`BroadcastBroadcastId`, `BroadcastGuildSnowflake`, `BroadcastChannelSnowflake`);



-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
