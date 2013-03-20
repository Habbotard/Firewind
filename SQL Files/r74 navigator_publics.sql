/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50512
Source Host           : localhost:3306
Source Database       : mainframe

Target Server Type    : MYSQL
Target Server Version : 50512
File Encoding         : 65001

Date: 2011-06-07 22:39:36
*/

SET FOREIGN_KEY_CHECKS=0;

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
) ENGINE=MyISAM AUTO_INCREMENT=28 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of navigator_publics
-- ----------------------------
INSERT INTO `navigator_publics` VALUES ('9', '8', '0', 'Internet cafe', 'netcafe', 'internal', '9', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('10', '6', '0', 'The Park', 'park', 'internal', '10', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('12', '12', '0', 'Heaven\'s Beach Cafe', 'rooftop', 'internal', '12', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('13', '11', '0', 'Heaven\'s Beach Cafe 2', 'rooftop', 'internal', '13', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('14', '5', '0', 'Pizzeria Pepperoni', 'pizza', 'internal', '14', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('16', '7', '0', 'Star Lounge', 'star_lounge', 'internal', '16', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('17', '9', '0', 'Mainostila: Hyytelopalatsi', 'officialrooms_nl/officialroom_sport.gif', 'external', '17', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('18', '1', '0', 'Newbie Lobby', 'lobby', 'internal', '7', '0', '24', '1', '0', '1');
INSERT INTO `navigator_publics` VALUES ('19', '10', '0', 'VG Library', 'library', 'internal', '8', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('20', '3', '0', 'Picnic Area', 'picnic', 'internal', '19', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('21', '4', '0', 'Theatredrome', 'theater', 'internal', '20', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('22', '7', '0', 'Info Bus', 'officialrooms_hq/hpv_infobus_nav_preview.png', 'internal', '21', '0', '24', '1', '0', '0');
INSERT INTO `navigator_publics` VALUES ('23', '2', '0', 'Habbo Cinema', 'habbo_cinema', 'internal', '22', '0', '24', '1', '0', '0');
