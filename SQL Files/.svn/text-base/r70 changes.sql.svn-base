/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50512
Source Host           : localhost:3306
Source Database       : mainframe

Target Server Type    : MYSQL
Target Server Version : 50512
File Encoding         : 65001

Date: 2011-05-25 20:49:33
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `room_active`
-- ----------------------------
DROP TABLE IF EXISTS `room_active`;
CREATE TABLE `room_active` (
  `roomid` int(10) unsigned NOT NULL,
  `active_users` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`roomid`),
  UNIQUE KEY `roomid` (`roomid`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of room_active
-- ----------------------------

-- ----------------------------
-- Table structure for `user_online`
-- ----------------------------
DROP TABLE IF EXISTS `user_online`;
CREATE TABLE `user_online` (
  `userid` int(10) unsigned NOT NULL,
  PRIMARY KEY (`userid`),
  UNIQUE KEY `userid` (`userid`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of user_online
-- ----------------------------

-- ----------------------------
-- Table structure for `user_tickets`
-- ----------------------------
DROP TABLE IF EXISTS `user_tickets`;
CREATE TABLE `user_tickets` (
  `userid` int(10) unsigned NOT NULL,
  `sessionticket` varchar(70) NOT NULL,
  PRIMARY KEY (`userid`,`sessionticket`),
  UNIQUE KEY `index_sso` (`sessionticket`) USING BTREE,
  UNIQUE KEY `index_id` (`userid`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of user_tickets
-- ----------------------------
