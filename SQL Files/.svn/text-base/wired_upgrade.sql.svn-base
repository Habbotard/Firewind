/*
Navicat MySQL Data Transfer

Source Server         : Local
Source Server Version : 50133
Source Host           : localhost:3306
Source Database       : butterfly

Target Server Type    : MYSQL
Target Server Version : 50133
File Encoding         : 65001

Date: 2011-09-20 22:31:25
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `trigger_in_place`
-- ----------------------------
DROP TABLE IF EXISTS `trigger_in_place`;
CREATE TABLE `trigger_in_place` (
  `original_trigger` int(11) NOT NULL,
  `triggers_item` int(11) NOT NULL,
  PRIMARY KEY (`original_trigger`,`triggers_item`),
  KEY `triggers_item` (`triggers_item`) USING HASH
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of trigger_in_place
-- ----------------------------

-- ----------------------------
-- Table structure for `trigger_item`
-- ----------------------------
DROP TABLE IF EXISTS `trigger_item`;
CREATE TABLE `trigger_item` (
  `trigger_id` int(11) NOT NULL,
  `trigger_input` enum('integer','multi_furniture','static_trigger','string') NOT NULL,
  `trigger_data_2` varchar(10) DEFAULT NULL,
  `trigger_data` varchar(100) DEFAULT NULL,
  `all_user_triggerable` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`trigger_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of trigger_item
-- ----------------------------

-- ----------------------------
-- Table structure for `trigger_rotation`
-- ----------------------------
DROP TABLE IF EXISTS `trigger_rotation`;
CREATE TABLE `trigger_rotation` (
  `item_id` int(11) NOT NULL,
  `rotation_status` tinyint(4) NOT NULL,
  `movement_status` tinyint(4) NOT NULL,
  PRIMARY KEY (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of trigger_rotation
-- ----------------------------
