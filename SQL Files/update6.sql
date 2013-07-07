SET FOREIGN_KEY_CHECKS=0;

DROP TABLE IF EXISTS `user_bots`;
CREATE TABLE `user_bots` (
  `id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `name` varchar(32) NOT NULL,
  `gender` char(255) NOT NULL,
  `figure` varchar(64) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk1` (`user_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

ALTER TABLE items_base CHANGE is_walkable can_walk tinyint(1); 