ALTER TABLE rooms DROP roomtype;
ALTER TABLE rooms DROP public_ccts;

ALTER TABLE room_models_customs DROP poolmap;
ALTER TABLE room_models DROP poolmap;
ALTER TABLE room_models DROP public_items;

DROP TABLE room_model_static;