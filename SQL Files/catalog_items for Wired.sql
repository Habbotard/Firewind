/*
Navicat MySQL Data Transfer

Source Server         : mainframe @ localhost
Source Server Version : 50515
Source Host           : localhost:3306
Source Database       : mainframe

Target Server Type    : MYSQL
Target Server Version : 50515
File Encoding         : 65001

Date: 2011-09-19 19:43:16
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `catalog_items`
-- ----------------------------

CREATE TABLE `catalog_items` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `page_id` int(11) NOT NULL,
  `item_ids` int(120) unsigned NOT NULL,
  `catalog_name` varchar(100) NOT NULL,
  `cost_credits` int(11) NOT NULL,
  `cost_pixels` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `cost_crystal` int(11) NOT NULL,
  `cost_oude_belcredits` int(11) NOT NULL,
  `song_id` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=3569018 DEFAULT CHARSET=latin1;

INSERT INTO `catalog_items` VALUES ('3568323', '177', '3568337', 'wf_trg_at_given_time', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568324', '177', '3568338', 'wf_trg_enter_room', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568325', '177', '3568339', 'wf_trg_game_ends', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568326', '177', '3568340', 'wf_trg_game_starts', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568327', '177', '3568341', 'wf_trg_periodically', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568328', '177', '3568342', 'wf_trg_says_something', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568329', '177', '3568343', 'wf_trg_score_achieved', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568330', '177', '3568344', 'wf_trg_state_changed', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568331', '177', '3568345', 'wf_trg_walks_off_furni', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568332', '177', '3568346', 'wf_trg_walks_on_furni', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568333', '178', '3568347', 'wf_act_give_score', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568334', '178', '3568348', 'wf_act_match_to_sshot', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568335', '178', '3568349', 'wf_act_move_rotate', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568336', '178', '3568350', 'wf_act_reset_timers', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568337', '178', '3568351', 'wf_act_show_message', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568338', '178', '3568352', 'wf_act_teleport_to', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568339', '178', '3568353', 'wf_act_toggle_state', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568340', '180', '3568354', 'wf_cnd_furnis_hv_avtrs', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568341', '180', '3568355', 'wf_cnd_match_snapshot', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568342', '180', '3568356', 'wf_cnd_time_less_than', '1', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568343', '180', '3568357', 'wf_cnd_time_more_than', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568344', '180', '3568358', 'wf_cnd_trggrer_on_frn', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568345', '179', '3568359', 'wf_arrowplate', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568346', '179', '3568360', 'wf_pressureplate', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568347', '179', '3568361', 'wf_ringplate', '1', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568348', '179', '3568362', 'wf_colortile', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568349', '179', '3568363', 'wf_colorwheel', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568350', '179', '3568364', 'wf_floor_switch1', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568351', '179', '3568365', 'wf_floor_switch2', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568352', '179', '3568366', 'wf_firegate', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568353', '179', '3568367', 'wf_glassdoor', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568354', '179', '3568368', 'wf_xtra_random', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568355', '179', '3568369', 'wf_xtra_unseen', '3', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568356', '179', '3568374', 'wf_wire1', '1', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568357', '179', '3568374', 'wf_wire1', '10', '0', '5', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568358', '179', '3568375', 'wf_wire2', '1', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568359', '179', '3568375', 'wf_wire2', '10', '0', '5', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568360', '179', '3568376', 'wf_wire3', '1', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568361', '179', '3568376', 'wf_wire3', '10', '0', '5', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568362', '179', '3568377', 'wf_wire4', '1', '0', '1', '0', '0', '0');
INSERT INTO `catalog_items` VALUES ('3568363', '179', '3568377', 'wf_wire4', '10', '0', '5', '0', '0', '0');


-- ----------------------------
-- Table structure for `catalog_pages`
-- ----------------------------

CREATE TABLE `catalog_pages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `parent_id` int(11) NOT NULL DEFAULT '-1',
  `caption` varchar(100) NOT NULL,
  `icon_color` int(11) NOT NULL DEFAULT '1',
  `icon_image` int(11) NOT NULL DEFAULT '1',
  `visible` enum('0','1') NOT NULL DEFAULT '1',
  `enabled` enum('0','1') NOT NULL DEFAULT '1',
  `min_rank` int(10) unsigned NOT NULL DEFAULT '1',
  `club_only` enum('0','1') NOT NULL DEFAULT '0',
  `coming_soon` enum('0','1') NOT NULL DEFAULT '0',
  `order_num` int(11) NOT NULL,
  `page_layout` enum('default_3x3','frontpage','spaces','recycler','recycler_info','recycler_prizes','trophies','plasto','marketplace','marketplace_own_items','pets','pets2','spaces_new','soundmachine') NOT NULL DEFAULT 'default_3x3',
  `page_headline` text NOT NULL,
  `page_teaser` text NOT NULL,
  `page_special` text NOT NULL,
  `page_text1` text NOT NULL,
  `page_text2` text NOT NULL,
  `page_text_details` text NOT NULL,
  `page_text_teaser` text NOT NULL,
  `text3` text NOT NULL,
  `link` text NOT NULL,
  `mobile_frntpg` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=916 DEFAULT CHARSET=latin1;

INSERT INTO `catalog_pages` VALUES ('177', '903', 'Inneschi', '1', '81', '1', '1', '1', '0', '0', '0', 'default_3x3', 'catalog_wired_header2_es', 'ctlg_pic_wired_triggers', '', '¿Qué provoca que usted puede determinar lo que debe suceder a la función está activada? Disparo programable mediante la colocación en el piso y la sala haciendo doble clic en el disparador. Ajuste la función en el botón del obturador, para que trabajen juntos.', '', '', '', '', '', '');
INSERT INTO `catalog_pages` VALUES ('178', '903', 'Effetti', '1', '82', '1', '1', '1', '0', '0', '0', 'default_3x3', 'catalog_wired_header3_es', 'ctlg_pic_wired_effects', '', 'Esta función determina lo que sucede cuando el obturador se activa. función programable mediante la colocación en el piso y la sala haciendo doble clic en la materia. Ajuste la función en el botón del obturador, para que trabajen juntos.', '', '', '', '', '', '');
INSERT INTO `catalog_pages` VALUES ('179', '903', 'Complementos', '1', '85', '1', '1', '1', '0', '0', '0', 'default_3x3', 'catalog_wired_header5_es', 'ctlg_pic_wired_support_furniture', '', 'Estos productos, que puede sintonizar con cable de su habitación! Los productos son artículos de primera necesidad, que puede agregar funcionalidad adicional mediante la combinación de los factores desencadenantes, las funciones y condiciones.', '', '', '', '', '', '');
INSERT INTO `catalog_pages` VALUES ('180', '903', 'Condizioni', '1', '83', '1', '1', '1', '0', '0', '0', 'default_3x3', 'catalog_wired_header4_es', 'ctlg_pic_wired_effects', '', '', '', '', '', '', '', '');
INSERT INTO `catalog_pages` VALUES ('903', '-1', 'Wired', '1', '80', '1', '1', '5', '0', '0', '0', 'default_3x3', 'head_shop', '', '', '', '', '', '', '', '', '');

CREATE TABLE `furniture` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `public_name` text NOT NULL COMMENT 'temp only',
  `item_name` varchar(100) NOT NULL,
  `type` enum('s','i','e','h') NOT NULL DEFAULT 's',
  `width` int(11) NOT NULL DEFAULT '1',
  `length` int(11) NOT NULL DEFAULT '1',
  `stack_height` double NOT NULL DEFAULT '1',
  `can_stack` enum('0','1') NOT NULL DEFAULT '1',
  `can_sit` enum('0','1') NOT NULL DEFAULT '0',
  `is_walkable` enum('0','1') NOT NULL DEFAULT '0',
  `sprite_id` int(11) NOT NULL,
  `allow_recycle` enum('0','1') NOT NULL DEFAULT '1',
  `allow_trade` enum('0','1') NOT NULL DEFAULT '1',
  `allow_marketplace_sell` enum('0','1') NOT NULL DEFAULT '1',
  `allow_gift` enum('0','1') NOT NULL DEFAULT '1',
  `allow_inventory_stack` enum('0','1') NOT NULL DEFAULT '1',
  `interaction_type` enum('default','gate','postit','roomeffect','dimmer','trophy','bed','scoreboard','vendingmachine','alert','onewaygate','loveshuffler','habbowheel','dice','bottle','teleport','rentals','pet','pool','roller','fbgate','lowpool','haloweenpool','normalskates','iceskates','pet0','pet1','pet2','pet3','pet4','pet5','pet6','pet7','pet8','pet9','pet10','pet11','pet12','football','footballgoalred','footballgoalblue','footballgoalyellow','footballgoalgreen','footballcountered','footballcountergreen','footballcounterblue','footballcounteryellow','banzaigateblue','banzaigatered','banzaigateyellow','banzaigategreen','banzaifloor','banzaiscoreblue','banzaiscorered','banzaiscoreyellow','banzaiscoregreen','banzaicounter','banzaitele','banzaipuck','banzaipyramid','freezetimer','freezeexit','freezeredcounter','freezebluecounter','freezeyellowcounter','freezegreencounter','freezeyellowgate','freezeredgate','freezegreengate','freezebluegate','freezetileblock','freezetile','musicdisc','triggertimer','triggerroomenter','triggergameend','triggergamestart','triggerrepeater','triggeronusersay','triggerscoreachieved','triggerstatechanged','triggerwalkonfurni','triggerwalkofffurni','actiongivescore','actionposreset','actionmoverotate','actionresettimer','actionshowmessage','actionteleportto','actiontogglestate','conditionfurnishaveusers','conditionstatepos','conditiontimelessthan','conditiontimemorethan','conditiontriggeronfurni','arrowplate','preassureplate','ringplate','colortile','colorwheel','floorswitch1','floorswitch2','firegate','glassfoor','specialrandom','specialunseen','wire') NOT NULL DEFAULT 'default',
  `interaction_modes_count` int(11) NOT NULL DEFAULT '1',
  `vending_ids` varchar(100) NOT NULL DEFAULT '0',
  `foot_figure` char(2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=4568441 DEFAULT CHARSET=latin1;

INSERT INTO `furniture` VALUES ('3568337', 'wf_trg_at_given_time', 'wf_trg_at_given_time', 's', '1', '1', '0.5', '1', '0', '0', '3679', '1', '1', '1', '1', '1', 'triggertimer', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568338', 'wf_trg_enter_room', 'wf_trg_enter_room', 's', '1', '1', '0.5', '1', '0', '0', '3683', '1', '1', '1', '1', '1', 'triggerroomenter', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568339', 'wf_trg_game_ends', 'wf_trg_game_ends', 's', '1', '1', '0.5', '1', '0', '0', '3680', '1', '1', '1', '1', '1', 'triggergameend', '5', '0', '');
INSERT INTO `furniture` VALUES ('3568340', 'wf_trg_game_starts', 'wf_trg_game_starts', 's', '1', '1', '0.5', '1', '0', '0', '3702', '1', '1', '1', '1', '1', 'triggergamestart', '5', '0', '');
INSERT INTO `furniture` VALUES ('3568341', 'wf_trg_periodically', 'wf_trg_periodically', 's', '1', '1', '0.5', '1', '0', '0', '3671', '1', '1', '1', '1', '1', 'triggerrepeater', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568342', 'wf_trg_says_something', 'wf_trg_says_something', 's', '1', '1', '0.5', '1', '0', '0', '3675', '1', '1', '1', '1', '1', 'triggeronusersay', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568343', 'wf_trg_score_achieved', 'wf_trg_score_achieved', 's', '1', '1', '0.5', '1', '0', '0', '3673', '1', '1', '1', '1', '1', 'triggerscoreachieved', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568344', 'wf_trg_state_changed', 'wf_trg_state_changed', 's', '1', '1', '0.5', '1', '0', '0', '3668', '1', '1', '1', '1', '1', 'triggerstatechanged', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568345', 'wf_trg_walks_off_furni', 'wf_trg_walks_off_furni', 's', '1', '1', '0.5', '1', '0', '0', '3678', '1', '1', '1', '1', '1', 'triggerwalkofffurni', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568346', 'wf_trg_walks_on_furni', 'wf_trg_walks_on_furni', 's', '1', '1', '0.5', '1', '0', '0', '3703', '1', '1', '1', '1', '1', 'triggerwalkonfurni', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568347', 'wf_act_give_score', 'wf_act_give_score', 's', '1', '1', '0.5', '1', '0', '0', '3697', '1', '1', '1', '1', '1', 'actiongivescore', '5', '0', '');
INSERT INTO `furniture` VALUES ('3568348', 'wf_act_match_to_sshot', 'wf_act_match_to_sshot', 's', '1', '1', '0.5', '1', '0', '0', '3700', '1', '1', '1', '1', '1', 'actionposreset', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568349', 'wf_act_move_rotate', 'wf_act_move_rotate', 's', '1', '1', '0.5', '1', '0', '0', '3663', '1', '1', '1', '1', '1', 'actionmoverotate', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568350', 'wf_act_reset_timers', 'wf_act_reset_timers', 's', '1', '1', '0.5', '1', '0', '0', '3691', '1', '1', '1', '1', '1', 'actionresettimer', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568351', 'wf_act_show_message', 'wf_act_show_message', 's', '1', '1', '0.5', '1', '0', '0', '3681', '1', '1', '1', '1', '1', 'actionshowmessage', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568352', 'wf_act_teleport_to', 'wf_act_teleport_to', 's', '1', '1', '0.5', '1', '0', '0', '3674', '1', '1', '1', '1', '1', 'actionteleportto', '1', '0', '');
INSERT INTO `furniture` VALUES ('3568353', 'wf_act_toggle_state', 'wf_act_toggle_state', 's', '1', '1', '0.5', '1', '0', '0', '3685', '1', '1', '1', '1', '1', 'actiontogglestate', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568354', 'wf_cnd_furnis_hv_avtrs', 'wf_cnd_furnis_hv_avtrs', 's', '1', '1', '0.5', '1', '0', '0', '3692', '1', '1', '1', '1', '1', 'conditionfurnishaveusers', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568355', 'wf_cnd_match_snapshot', 'wf_cnd_match_snapshot', 's', '1', '1', '0.5', '1', '0', '0', '3695', '1', '1', '1', '1', '1', 'conditionstatepos', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568356', 'wf_cnd_time_less_than', 'wf_cnd_time_less_than', 's', '1', '1', '0.5', '1', '0', '0', '3682', '1', '1', '1', '1', '1', 'conditiontimelessthan', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568357', 'wf_cnd_time_more_than', 'wf_cnd_time_more_than', 's', '1', '1', '0.5', '1', '0', '0', '3665', '1', '1', '1', '1', '1', 'conditiontimemorethan', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568358', 'wf_cnd_trggrer_on_frn', 'wf_cnd_trggrer_on_frn', 's', '1', '1', '0.5', '1', '0', '0', '3694', '1', '1', '1', '1', '1', 'conditiontriggeronfurni', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568359', 'wf_arrowplate', 'wf_arrowplate', 's', '1', '1', '0', '1', '0', '1', '3693', '1', '1', '1', '1', '1', 'arrowplate', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568360', 'wf_pressureplate', 'wf_pressureplate', 's', '1', '1', '0', '1', '0', '1', '3688', '1', '1', '1', '1', '1', 'preassureplate', '2', '0', '');
INSERT INTO `furniture` VALUES ('3568361', 'wf_ringplate', 'wf_ringplate', 's', '1', '1', '0', '1', '0', '1', '3687', '1', '1', '1', '1', '1', 'ringplate', '2', '0', '');
INSERT INTO `furniture` VALUES ('3568362', 'wf_colortile', 'wf_colortile', 's', '1', '1', '0', '1', '0', '1', '3666', '1', '1', '1', '1', '1', 'colortile', '10', '0', '');
INSERT INTO `furniture` VALUES ('3568363', 'wf_colorwheel', 'wf_colorwheel', 's', '1', '1', '1', '1', '0', '0', '3676', '1', '1', '1', '1', '1', 'colorwheel', '10', '0', '');
INSERT INTO `furniture` VALUES ('3568364', 'wf_floor_switch1', 'wf_floor_switch1', 's', '1', '1', '1', '1', '0', '0', '3701', '1', '1', '1', '1', '1', 'floorswitch1', '2', '0', '');
INSERT INTO `furniture` VALUES ('3568365', 'wf_floor_switch2', 'wf_floor_switch2', 's', '1', '1', '1', '1', '0', '0', '3667', '1', '1', '1', '1', '1', 'floorswitch2', '5', '0', '');
INSERT INTO `furniture` VALUES ('3568366', 'wf_firegate', 'wf_firegate', 's', '1', '1', '0', '1', '0', '1', '3686', '1', '1', '1', '1', '1', 'firegate', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568367', 'wf_glassdoor', 'wf_glassdoor', 's', '1', '1', '0.1', '1', '0', '0', '3699', '1', '1', '1', '1', '1', 'gate', '3', '0', '');
INSERT INTO `furniture` VALUES ('3568368', 'wf_xtra_random', 'wf_xtra_random', 's', '1', '1', '0.5', '1', '0', '1', '3669', '1', '1', '1', '1', '1', 'specialrandom', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568369', 'wf_xtra_unseen', 'wf_xtra_unseen', 's', '1', '1', '0.5', '1', '0', '1', '3670', '1', '1', '1', '1', '1', 'specialunseen', '4', '0', '');
INSERT INTO `furniture` VALUES ('3568374', 'wf_wire1', 'wf_wire1', 's', '1', '1', '0', '1', '0', '1', '3696', '1', '1', '1', '1', '1', 'wire', '2', '0', '');
INSERT INTO `furniture` VALUES ('3568375', 'wf_wire2', 'wf_wire2', 's', '1', '1', '0', '1', '0', '1', '3664', '1', '1', '1', '1', '1', 'wire', '2', '0', '');
INSERT INTO `furniture` VALUES ('3568376', 'wf_wire3', 'wf_wire3', 's', '1', '1', '0', '1', '0', '1', '3698', '1', '1', '1', '1', '1', 'wire', '2', '0', '');
INSERT INTO `furniture` VALUES ('3568377', 'wf_wire4', 'wf_wire4', 's', '1', '1', '0', '1', '0', '1', '3677', '1', '1', '1', '1', '1', 'wire', '2', '0', '');