/*
Navicat MySQL Data Transfer

Source Server         : mainframe @ localhost
Source Server Version : 50515
Source Host           : localhost:3306
Source Database       : mainframe

Target Server Type    : MYSQL
Target Server Version : 50515
File Encoding         : 65001

Date: 2011-10-15 22:04:56
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `achievements`
-- ----------------------------
DROP TABLE IF EXISTS `achievements`;
CREATE TABLE `achievements` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `group_name` varchar(64) NOT NULL DEFAULT 'ACH_',
  `category` varchar(255) NOT NULL DEFAULT 'identity',
  `level` int(11) NOT NULL DEFAULT '1',
  `reward_pixels` int(11) NOT NULL DEFAULT '100',
  `reward_points` int(11) DEFAULT '10',
  `progress_needed` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of achievements
-- ----------------------------
INSERT INTO `achievements` VALUES ('1', 'ACH_AvatarTags', 'identity', '1', '5', '5', '5');
INSERT INTO `achievements` VALUES ('2', 'ACH_AvatarLooks', 'identity', '1', '5', '5', '1');
INSERT INTO `achievements` VALUES ('3', 'ACH_Motto', 'identity', '1', '10', '10', '1');
INSERT INTO `achievements` VALUES ('4', 'ACH_EmailVerification', 'identity', '1', '20', '20', '1');
INSERT INTO `achievements` VALUES ('5', 'ACH_RespectGiven', 'social', '1', '100', '100', '100');
INSERT INTO `achievements` VALUES ('6', 'ACH_BasicClub', 'identity', '1', '25', '25', '1');
INSERT INTO `achievements` VALUES ('7', 'ACH_VipClub', 'identity', '1', '50', '50', '1');
INSERT INTO `achievements` VALUES ('8', 'ACH_BasicClub', 'identity', '2', '100', '100', '3');
INSERT INTO `achievements` VALUES ('9', 'ACH_BasicClub', 'identity', '3', '250', '250', '6');
INSERT INTO `achievements` VALUES ('10', 'ACH_BasicClub', 'identity', '4', '350', '350', '9');
INSERT INTO `achievements` VALUES ('11', 'ACH_BasicClub', 'identity', '5', '500', '500', '12');
INSERT INTO `achievements` VALUES ('12', 'ACH_VipClub', 'identity', '2', '200', '200', '3');
INSERT INTO `achievements` VALUES ('13', 'ACH_VipClub', 'identity', '3', '500', '500', '6');
INSERT INTO `achievements` VALUES ('14', 'ACH_VipClub', 'identity', '4', '700', '700', '9');
INSERT INTO `achievements` VALUES ('15', 'ACH_VipClub', 'identity', '5', '1000', '1000', '12');
INSERT INTO `achievements` VALUES ('16', 'ACH_RespectEarned', 'social', '1', '5', '5', '1');
INSERT INTO `achievements` VALUES ('17', 'ACH_RespectEarned', 'social', '2', '20', '20', '10');
INSERT INTO `achievements` VALUES ('18', 'ACH_RespectEarned', 'social', '3', '40', '40', '20');
INSERT INTO `achievements` VALUES ('19', 'ACH_RespectEarned', 'social', '4', '100', '100', '50');
INSERT INTO `achievements` VALUES ('20', 'ACH_RespectEarned', 'social', '5', '200', '200', '100');
INSERT INTO `achievements` VALUES ('21', 'ACH_RespectEarned', 'social', '6', '500', '500', '250');
INSERT INTO `achievements` VALUES ('22', 'ACH_RespectEarned', 'social', '7', '1000', '1000', '500');
INSERT INTO `achievements` VALUES ('23', 'ACH_RespectEarned', 'social', '8', '1300', '1300', '750');
INSERT INTO `achievements` VALUES ('24', 'ACH_RespectEarned', 'social', '9', '2000', '2000', '1000');
INSERT INTO `achievements` VALUES ('25', 'ACH_RespectEarned', 'social', '10', '3000', '3000', '1500');

-- ----------------------------
-- Table structure for `quests`
-- ----------------------------
DROP TABLE IF EXISTS `quests`;
CREATE TABLE `quests` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `category` varchar(32) NOT NULL DEFAULT '',
  `series_number` int(11) NOT NULL DEFAULT '0',
  `goal_type` int(10) NOT NULL DEFAULT '0',
  `goal_data` int(10) unsigned NOT NULL DEFAULT '0',
  `name` varchar(32) NOT NULL DEFAULT '',
  `reward` int(11) NOT NULL DEFAULT '10',
  `data_bit` varchar(2) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of quests
-- ----------------------------
INSERT INTO `quests` VALUES ('1', 'room_builder', '1', '0', '3', 'MOVEITEM', '10', '_2');
INSERT INTO `quests` VALUES ('2', 'social', '1', '8', '1', 'ENTEROTHERSROOM', '10', '_2');
INSERT INTO `quests` VALUES ('3', 'identity', '1', '14', '1', 'CHANGEFIGURE', '10', '_2');
INSERT INTO `quests` VALUES ('4', 'explore', '1', '17', '1936', 'FINDLIFEGUARDTOWER', '10', '_2');
INSERT INTO `quests` VALUES ('5', 'room_builder', '2', '1', '3', 'ROTATEITEM', '10', '');
INSERT INTO `quests` VALUES ('6', 'room_builder', '3', '2', '1', 'PLACEITEM', '10', '');
INSERT INTO `quests` VALUES ('7', 'room_builder', '4', '3', '1', 'PICKUPITEM', '10', '');
INSERT INTO `quests` VALUES ('8', 'room_builder', '5', '4', '2', 'SWITCHSTATE', '10', '');
INSERT INTO `quests` VALUES ('9', 'room_builder', '6', '5', '1', 'STACKITEM', '10', '');
INSERT INTO `quests` VALUES ('10', 'room_builder', '7', '6', '1', 'PLACEFLOOR', '10', '');
INSERT INTO `quests` VALUES ('11', 'room_builder', '8', '7', '1', 'PLACEWALLPAPER', '10', '_1');
INSERT INTO `quests` VALUES ('12', 'identity', '2', '15', '1', 'CHANGEMOTTO', '10', '');
INSERT INTO `quests` VALUES ('13', 'identity', '3', '16', '1', 'WEARBADGE', '10', '');
INSERT INTO `quests` VALUES ('14', 'social', '2', '9', '1', 'CHATWITHSOMEONE', '10', '');
INSERT INTO `quests` VALUES ('15', 'social', '3', '10', '1', 'REQUESTFRIEND', '10', '');
INSERT INTO `quests` VALUES ('16', 'social', '4', '11', '1', 'GIVERESPECT', '10', '');
INSERT INTO `quests` VALUES ('17', 'social', '5', '12', '1', 'DANCE', '10', '');
INSERT INTO `quests` VALUES ('18', 'social', '6', '13', '1', 'WAVE', '10', '');
INSERT INTO `quests` VALUES ('19', 'explore', '2', '17', '1948', 'SWIM', '10', '');
INSERT INTO `quests` VALUES ('20', 'explore', '3', '17', '1969', 'FINDSURFBOARD', '10', '');
INSERT INTO `quests` VALUES ('21', 'explore', '4', '17', '1956', 'FINDBEETLE', '10', '');
INSERT INTO `quests` VALUES ('22', 'explore', '5', '17', '1369', 'FINDNEONFLOOR', '10', '');
INSERT INTO `quests` VALUES ('23', 'explore', '6', '17', '1375', 'FINDDISCOBALL', '10', '');
INSERT INTO `quests` VALUES ('24', 'explore', '7', '17', '1019', 'FINDJUKEBOX', '10', '');
INSERT INTO `quests` VALUES ('25', 'explore', '8', '17', '2050', 'FINDBBGATE', '10', '');
INSERT INTO `quests` VALUES ('26', 'explore', '9', '17', '2040', 'FINDBBTILE', '10', '');
INSERT INTO `quests` VALUES ('27', 'explore', '10', '17', '2049', 'FINDBBTELEPORT', '10', '');
INSERT INTO `quests` VALUES ('28', 'explore', '11', '17', '2167', 'FINDFREEZEGATE', '10', '');
INSERT INTO `quests` VALUES ('29', 'explore', '12', '17', '2172', 'FINDFREEZESCOREBOARD', '10', '');
INSERT INTO `quests` VALUES ('30', 'explore', '13', '17', '2166', 'FINDFREEZEEXITTILE', '10', '');
INSERT INTO `quests` VALUES ('31', 'explore', '14', '17', '1413', 'ICESKATE', '10', '');
INSERT INTO `quests` VALUES ('32', 'explore', '15', '17', '2148', 'FINDTAGPOLE', '10', '');
INSERT INTO `quests` VALUES ('33', 'explore', '16', '17', '2199', 'ROLLERSKATE', '10', '');

-- ----------------------------
-- Table structure for `user_achievement`
-- ----------------------------
DROP TABLE IF EXISTS `user_achievement`;
CREATE TABLE `user_achievement` (
  `userid` int(11) unsigned NOT NULL,
  `group` varchar(255) NOT NULL,
  `level` int(11) NOT NULL,
  `progress` int(11) NOT NULL,
  PRIMARY KEY (`userid`,`group`),
  KEY `id` (`userid`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of user_achievement
-- ----------------------------
INSERT INTO `user_achievement` VALUES ('1', 'ACH_AvatarLooks', '1', '0');

-- ----------------------------
-- Table structure for `user_quests`
-- ----------------------------
DROP TABLE IF EXISTS `user_quests`;
CREATE TABLE `user_quests` (
  `user_id` int(10) unsigned NOT NULL,
  `quest_id` int(10) unsigned NOT NULL,
  `progress` int(10) NOT NULL,
  PRIMARY KEY (`user_id`,`quest_id`),
  KEY `userid` (`user_id`) USING HASH,
  KEY `questid` (`quest_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of user_quests
-- ----------------------------
INSERT INTO `user_quests` VALUES ('1', '1', '3');
INSERT INTO `user_quests` VALUES ('1', '5', '3');
INSERT INTO `user_quests` VALUES ('1', '6', '1');
INSERT INTO `user_quests` VALUES ('1', '7', '1');
INSERT INTO `user_quests` VALUES ('1', '8', '2');
INSERT INTO `user_quests` VALUES ('1', '9', '1');
INSERT INTO `user_quests` VALUES ('1', '10', '0');

ALTER TABLE users ADD achievement_points int NOT NULL;