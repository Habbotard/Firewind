/*
Navicat MySQL Data Transfer

Source Server         : Local
Source Server Version : 50133
Source Host           : localhost:3306
Source Database       : butterfly

Target Server Type    : MYSQL
Target Server Version : 50133
File Encoding         : 65001

Date: 2011-10-25 18:44:12
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `room_swearword_filter`
-- ----------------------------
DROP TABLE IF EXISTS `room_swearword_filter`;
CREATE TABLE `room_swearword_filter` (
  `word` varchar(200) NOT NULL,
  PRIMARY KEY (`word`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
