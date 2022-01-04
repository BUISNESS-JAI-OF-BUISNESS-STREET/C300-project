DROP TABLE IF EXISTS QuizQuestionBindDB;
DROP TABLE IF EXISTS Question;
DROP TABLE IF EXISTS Quiz;
DROP TABLE IF EXISTS Account;
DROP TABLE IF EXISTS Result;


CREATE TABLE Account (
    AccountId       VARCHAR(50)    NOT NULL PRIMARY KEY,
    Name     VARCHAR (50)   NOT NULL,
    Password VARBINARY (50) NOT NULL,
    Role     VARCHAR(50)    NOT NULL,
    
);
GO

INSERT INTO Account (AccountId, Name, Password, Role) VALUES 
('jonlee', 'Jonathan Lee', HASHBYTES('SHA1','password'),'Admin'),
('felkang', 'Felicia Kang', HASHBYTES('SHA1','password'),'User'),
('dorta', 'Dorothy Ta', HASHBYTES('SHA1','password'),'User'),
('deslee', 'Desmond Lee', HASHBYTES('SHA1','password'),'Admin')
GO

CREATE TABLE Question (
	QuestionId       INT IDENTITY(1,1)  PRIMARY KEY,
	Questions	VARCHAR(150)	NOT NULL,
	FirstOption	VARCHAR(50)	NOT NULL,
	SecondOption	VARCHAR(50)	NOT NULL,
	ThirdOption	VARCHAR(50)	NOT NULL,
	FourthOption	VARCHAR(50)	NOT NULL,
	Topic 	VARCHAR(50)	NOT NULL,
	CorrectAns VARCHAR(50) NOT NULL,
Segment VARCHAR(50) NOT NULL,
    UserCode VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Account(AccountId)

);

SET IDENTITY_INSERT Question ON
INSERT INTO Question (QuestionId, Questions, FirstOption, SecondOption, ThirdOption, FourthOption, Topic, CorrectAns, UserCode, Segment)	VALUES
(1, 'The mountaineers face one obstacle after another; _____ they were undaunted and finally made it to the submit.', 'thus', 'Therefore', 'although', 'nevertheless','English','nevertheless','deslee', 'Grammar'),
(2,'_____ the heavy downpour, the road was flooded and many shops were forced to close.', 'Despite', 'Due to', 'Except For', 'Regardless of','English','Due to', 'deslee', 'Grammar'),
(3, 'On seeing the snake, Owen stood _____ on the spot, unable to take the next step.', 'freeze', 'frozen', 'froze', 'freezing','English','frozen', 'deslee', 'Grammar'),
(4, ' _____ of the children wants to go to the zoo this weekend.', 'None', 'Some', 'Many', 'A few','English','None', 'deslee', 'Grammar'),
(5, 'There is a line of ants that _____ from the dustbin and ends at the hole in the wall', 'start', 'starts', 'started', 'starting','English', 'starting', 'deslee', 'Grammar'),
(6, '2+2', '4', '0', '2', '1','Mathematics', '4', 'deslee', ''),
(7, '2*2', '4', '0', '2', '1','Mathematics', '4', 'deslee', ''),
(8, '2/2', '4', '0', '2', '1','Mathematics', '1','deslee', ''),
(9, '2-2', '4', '0', '2', '1','Mathematics', '0','deslee', ''),
(10, 'There is a line of ants that _____ from the dustbin and ends at the hole in the wall', 'start', 'starts', 'started', 'starting','English', 'starting', 'deslee', 'Grammar')
SET IDENTITY_INSERT Question OFF





CREATE TABLE Quiz (
QuizId INT IDENTITY (1, 1) PRIMARY KEY,
Title VARCHAR (150) NOT NULL,
Topic VARCHAR (50) NOT NULL,
Sec INT NOT NULL,
StartDt DATETIME NOT NULL,
EndDt DATETIME NOT NULL,
UserCode VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Account(AccountId)
);

GO

SET IDENTITY_INSERT Quiz ON
INSERT INTO Quiz (QuizId, Title, Topic, Sec, StartDt, EndDt, UserCode) VALUES 
(1, 'PREPARATORY COURSE FOR ADMISSION TO GOVERNMENT SCHOOLS (Secondary 2-3)','English', 2, '2021-11-17 08:00','2021-11-17 10:10', 'deslee'),
(2, 'SEC 2 MONTHLY TEST PAPER 1', 'Mathematics', 2, '2021-11-13 08:00' ,'2021-11-17 08:35', 'deslee'),
(3, 'SEC 2 MONTHLY TEST PAPER 2', 'Mathematics', 2, '2021-10-08 08:00','2021-11-17 08:50', 'deslee'),
(4, 'PREPARATORY COURSE FOR ADMISSION TO GOVERNMENT SCHOOLS (Secondary 2-3)', 'English', 2, '2021-11-17 08:00','2021-11-17 08:00', 'deslee')
SET IDENTITY_INSERT Quiz OFF
GO



CREATE TABLE Result (
    ResultId       INT  IDENTITY (1, 1) PRIMARY KEY,
    QuizId      INT NOT NULL,
    AccountId     VARCHAR (50)   NOT NULL,
    Name    VARCHAR (50) NOT NULL,
    Title	VARCHAR (150) NOT NULL,
    Topic     VARCHAR(50)    NOT NULL,
    Score     INT          NOT NULL,
    Attempt   BIT         NOT NULL,
    Dt DATETIME     NOT NULL
);

SET IDENTITY_INSERT Result ON
INSERT INTO Result (ResultId, QuizId ,AccountId, Name, Title, Topic, Score, Attempt, Dt) VALUES 
(1,  '1', 'jonlee', 'Johnathan Lee', 'PREPARATORY COURSE FOR ADMISSION TO GOVERNMENT SCHOOLS (Secondary 2-3)', 'English', 59, 1, '2021-11-17 08:00'),
(2,  '2', 'felkang', 'Felicia Kang', 'SEC 2 MONTHLY TEST PAPER 1', 'Mathematics', 70, 1, '2021-11-13 08:00'),
(3,  '3', 'dorta', 'Dorothy Ta', 'SEC 2 MONTHLY TEST PAPER 1', 'Mathematics', 89, 1, '2021-10-08 08:00'),
(4,  '4', 'deslee', 'Desmond Lee', 'PREPARATORY COURSE FOR ADMISSION TO GOVERNMENT SCHOOLS (Secondary 2-3)','English', 0, 0, '2020-11-04 08:00')
SET IDENTITY_INSERT Result OFF

CREATE TABLE QuizQuestionBindDB (
    ID              INT  IDENTITY (1, 1) PRIMARY KEY,
	QuestionId       INT NOT NULL FOREIGN KEY REFERENCES Question(QuestionId),
    QuizId      INT NOT NULL FOREIGN KEY REFERENCES Quiz(QuizId)

);

SET IDENTITY_INSERT QuizQuestionBindDB ON
INSERT INTO QuizQuestionBindDB (ID, QuestionId, QuizId) VALUES 
(1,'1','1'),
(2, '1','2'),
(3, '1','3')
SET IDENTITY_INSERT QuizQuestionBindDB OFF


