﻿
INSERT INTO USER_RIGHT_TABLE (NAME, CATEGORY, RIGHT_TYPE, RIGHT, IDX, COMMENTS) VALUES ('FusionLink', 'SDK Rights', 2, -1, (SELECT MAX(IDX)+1 FROM USER_RIGHT_TABLE), 'Access FusionLink');  