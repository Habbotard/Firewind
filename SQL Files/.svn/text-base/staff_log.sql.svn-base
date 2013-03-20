/*
Navicat MySQL Data Transfer

Source Server         : Sunnieday Netherlands SQL
Source Server Version : 50138
Source Host           : 216.245.200.178:3306
Source Database       : sunnieday_flash

Target Server Type    : MYSQL
Target Server Version : 50138
File Encoding         : 65001

Date: 2011-10-25 21:02:08
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `staff_log`
-- ----------------------------
DROP TABLE IF EXISTS `staff_log`;
CREATE TABLE `staff_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `staffuser` varchar(40) NOT NULL,
  `target` varchar(40) NOT NULL,
  `action_type` varchar(40) NOT NULL,
  `description` text,
  `extra_info` text,
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of staff_log
-- ----------------------------
