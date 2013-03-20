/*
Navicat MySQL Data Transfer

Source Server         : Localhost
Source Server Version : 50133
Source Host           : localhost:3306
Source Database       : butterfly

Target Server Type    : MYSQL
Target Server Version : 50133
File Encoding         : 65001

Date: 2011-05-25 23:24:39
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `user_tickets`
-- ----------------------------
DROP TABLE IF EXISTS `user_tickets`;
CREATE TABLE `user_tickets` (
  `userid` int(10) unsigned NOT NULL,
  `sessionticket` varchar(70) NOT NULL,
  `ipaddress` varchar(20) NOT NULL,
  PRIMARY KEY (`sessionticket`,`ipaddress`),
  UNIQUE KEY `index_id` (`userid`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `users` DROP `auth_ticket`
-- ----------------------------
-- Records of user_tickets
-- ----------------------------
