/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50512
Source Host           : localhost:3306
Source Database       : mainframe

Target Server Type    : MYSQL
Target Server Version : 50512
File Encoding         : 65001

Date: 2011-06-07 16:34:59
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `achievements`
-- ----------------------------
DROP TABLE IF EXISTS `achievements`;
CREATE TABLE `achievements` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `levels` int(11) NOT NULL DEFAULT '1',
  `dynamic_badgelevel` enum('0','1') NOT NULL DEFAULT '1',
  `badge` varchar(100) NOT NULL,
  `pixels_base` int(11) NOT NULL DEFAULT '50',
  `pixels_multiplier` double NOT NULL DEFAULT '1.25',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=12 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of achievements
-- ----------------------------
INSERT INTO `achievements` VALUES ('1', '1', '1', 'ACH_AvatarLooks', '50', '0');
INSERT INTO `achievements` VALUES ('5', '1', '1', 'ACH_Motto', '50', '0');
INSERT INTO `achievements` VALUES ('6', '1', '1', 'ACH_Student', '50', '0');
INSERT INTO `achievements` VALUES ('7', '1', '1', 'ACH_AvatarTags', '50', '0');
INSERT INTO `achievements` VALUES ('11', '1', '0', 'Z63', '50', '0');

-- ----------------------------
-- Table structure for `navigator_pubcats`
-- ----------------------------
DROP TABLE IF EXISTS `navigator_pubcats`;
CREATE TABLE `navigator_pubcats` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `caption` varchar(100) NOT NULL,
  `enabled` enum('0','1') NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of navigator_pubcats
-- ----------------------------
INSERT INTO `navigator_pubcats` VALUES ('1', 'Cafes and Restaurants', '1');
INSERT INTO `navigator_pubcats` VALUES ('2', 'Lounges and Entertainment', '1');
INSERT INTO `navigator_pubcats` VALUES ('3', 'Pubs and Clubs', '1');
INSERT INTO `navigator_pubcats` VALUES ('4', 'Outside Spaces & Swimming Pools', '1');

-- ----------------------------
-- Table structure for `navigator_publics`
-- ----------------------------
DROP TABLE IF EXISTS `navigator_publics`;
CREATE TABLE `navigator_publics` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ordernum` int(11) NOT NULL DEFAULT '1',
  `bannertype` enum('0','1') NOT NULL COMMENT '0 = big, 1 = normal',
  `caption` varchar(100) NOT NULL,
  `image` text NOT NULL,
  `image_type` enum('internal','external') NOT NULL DEFAULT 'internal',
  `room_id` int(10) unsigned NOT NULL,
  `category_id` int(11) NOT NULL DEFAULT '0',
  `category_parent_id` int(11) NOT NULL DEFAULT '-1',
  `enabled` enum('0','1') NOT NULL DEFAULT '1',
  `recommended` enum('0','1') NOT NULL DEFAULT '0',
  `category` enum('0','1') NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=24 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of navigator_publics
-- ----------------------------
INSERT INTO `navigator_publics` VALUES ('9', '8', '0', 'Internet cafe', 'netcafe', 'internal', '9', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('10', '6', '0', 'The Park', 'park', 'internal', '10', '0', '2', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('12', '12', '0', 'Heaven\'s Beach Cafe', 'rooftop', 'internal', '12', '0', '3', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('13', '11', '0', 'Heaven\'s Beach Cafe 2', 'rooftop', 'internal', '13', '0', '3', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('14', '5', '0', 'Pizzeria Pepperoni', 'pizza', 'internal', '14', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('16', '7', '0', 'Star Lounge', 'star_lounge', 'internal', '16', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('17', '9', '0', 'Mainostila: Hyytelopalatsi', 'officialrooms_nl/officialroom_sport.gif', 'external', '17', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('18', '1', '0', 'Newbie Lobby', 'lobby', 'internal', '7', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('19', '10', '0', 'VG Library', 'library', 'internal', '8', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('20', '3', '0', 'Picnic Area', 'picnic', 'internal', '19', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('21', '4', '0', 'Theatredrome', 'theater', 'internal', '20', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('22', '7', '0', 'Info Bus', 'officialrooms_hq/hpv_infobus_nav_preview.png', 'internal', '21', '0', '-1', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('23', '2', '0', 'Habbo Cinema', 'habbo_cinema', 'internal', '22', '0', '-1', '1', '0', '0');

-- ----------------------------
-- Table structure for `quests`
-- ----------------------------
DROP TABLE IF EXISTS `quests`;
CREATE TABLE `quests` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `type` varchar(100) NOT NULL,
  `action` varchar(100) NOT NULL,
  `needofcount` int(10) NOT NULL DEFAULT '1',
  `enabled` enum('0','1') NOT NULL DEFAULT '1',
  `level_num` int(10) NOT NULL DEFAULT '1',
  `pixel_reward` int(10) NOT NULL DEFAULT '50',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=25 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of quests
-- ----------------------------
INSERT INTO `quests` VALUES ('1', 'room_builder', 'MOVE_ITEM', '3', '1', '1', '50');
INSERT INTO `quests` VALUES ('2', 'identity', 'CHANGE_FIGURE', '1', '1', '1', '50');
INSERT INTO `quests` VALUES ('3', 'social', 'CHAT_WITH_SOMEONE', '1', '1', '6', '50');
INSERT INTO `quests` VALUES ('4', 'social', 'REQUEST_FRIEND', '1', '1', '2', '50');
INSERT INTO `quests` VALUES ('5', 'social', 'GIVE_RESPECT', '1', '1', '3', '50');
INSERT INTO `quests` VALUES ('7', 'room_builder', 'ROTATE_ITEM', '3', '1', '2', '50');
INSERT INTO `quests` VALUES ('6', 'social', 'DANCE', '1', '1', '5', '50');
INSERT INTO `quests` VALUES ('8', 'social', 'WAVE', '1', '1', '4', '50');
INSERT INTO `quests` VALUES ('9', 'room_builder', 'STACKITEM', '3', '1', '3', '50');
INSERT INTO `quests` VALUES ('10', 'room_builder', 'PICKUPITEM', '1', '1', '4', '50');
INSERT INTO `quests` VALUES ('11', 'room_builder', 'PLACEWALLPAPER', '1', '1', '7', '50');
INSERT INTO `quests` VALUES ('12', 'room_builder', 'SWITCHSTATE', '3', '1', '6', '50');
INSERT INTO `quests` VALUES ('13', 'room_builder', 'PLACEFLOOR', '1', '1', '8', '50');
INSERT INTO `quests` VALUES ('14', 'room_builder', 'PLACEITEM', '1', '1', '5', '50');
INSERT INTO `quests` VALUES ('15', 'social', 'ENTEROTHERSROOM', '1', '1', '1', '50');
INSERT INTO `quests` VALUES ('16', 'identity', 'WEARBADGE', '1', '1', '3', '50');
INSERT INTO `quests` VALUES ('17', 'identity', 'CHANGEMOTTO', '1', '1', '2', '50');
INSERT INTO `quests` VALUES ('18', 'explore', 'FINDLIFEGUARDTOWER', '1', '1', '1', '50');
INSERT INTO `quests` VALUES ('19', 'explore', 'SWIM', '1', '1', '2', '50');
INSERT INTO `quests` VALUES ('20', 'explore', 'FINDSURFBOARD', '1', '1', '3', '50');
INSERT INTO `quests` VALUES ('21', 'explore', 'FINDBEETLE', '1', '1', '4', '50');
INSERT INTO `quests` VALUES ('22', 'explore', 'FINDNEONFLOOR', '1', '1', '5', '50');
INSERT INTO `quests` VALUES ('23', 'explore', 'FINDDISCOBALL', '1', '1', '6', '50');
INSERT INTO `quests` VALUES ('24', 'explore', 'FINDJUKEBOX', '1', '1', '7', '50');

-- ----------------------------
-- Table structure for `user_quests`
-- ----------------------------
DROP TABLE IF EXISTS `user_quests`;
CREATE TABLE `user_quests` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(10) unsigned NOT NULL,
  `quest_id` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of user_quests
-- ----------------------------
