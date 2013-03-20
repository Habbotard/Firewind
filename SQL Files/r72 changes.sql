/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50512
Source Host           : localhost:3306
Source Database       : mainframe

Target Server Type    : MYSQL
Target Server Version : 50512
File Encoding         : 65001

Date: 2011-05-30 17:22:32
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `messenger_friendships`
-- ----------------------------
DROP TABLE IF EXISTS `messenger_friendships`;
CREATE TABLE `messenger_friendships` (
  `sender` int(10) unsigned NOT NULL,
  `receiver` int(10) unsigned NOT NULL,
  PRIMARY KEY (`sender`,`receiver`),
  KEY `i_sender` (`sender`) USING HASH,
  KEY `i_receiver` (`receiver`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of messenger_friendships
-- ----------------------------


-- ----------------------------
-- Table structure for `messenger_requests`
-- ----------------------------
DROP TABLE IF EXISTS `messenger_requests`;
CREATE TABLE `messenger_requests` (
  `sender` int(10) unsigned NOT NULL,
  `receiver` int(10) unsigned NOT NULL,
  PRIMARY KEY (`sender`,`receiver`),
  KEY `i_sender` (`sender`) USING HASH,
  KEY `i_receiver` (`receiver`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


ALTER TABLE `user_items` CHANGE `id` `id` INT( 9 ) UNSIGNED NOT NULL AUTO_INCREMENT 
-- ----------------------------
-- Records of messenger_requests
-- ----------------------------
