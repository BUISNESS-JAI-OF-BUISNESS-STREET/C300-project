DROP TABLE IF EXISTS QuizQuestionBindDB;
DROP TABLE IF EXISTS Announcement;
DROP TABLE IF EXISTS TeacherClassBindDB;
DROP TABLE IF EXISTS TeacherStudentBindDB;
DROP TABLE IF EXISTS StudentClassBindDB;
DROP TABLE IF EXISTS Question;
DROP TABLE IF EXISTS Quiz;
DROP TABLE IF EXISTS Result;
DROP TABLE IF EXISTS Teacher;
DROP TABLE IF EXISTS Class;
DROP TABLE IF EXISTS Student;
DROP TABLE IF EXISTS Account;

CREATE TABLE Account (
    AccountId       VARCHAR(50)    NOT NULL PRIMARY KEY,
    Name     VARCHAR (50)   NOT NULL,
    Password VARBINARY (50) NOT NULL,
    Role     VARCHAR(50)    NOT NULL
);

INSERT INTO Account (AccountId, Name, Password, Role) VALUES 
('jonlee', 'Jonathan Lee', HASHBYTES('SHA1','password'),'Admin'),
('felkang', 'Felicia Kang', HASHBYTES('SHA1','password'),'Admin'),
('dorta', 'Dorothy Ta', HASHBYTES('SHA1','password'),'Admin'),
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
(10, 'There is a line of ants that _____ from the dustbin and ends at the hole in the wall', 'start', 'starts', 'started', 'starting','English', 'starting', 'deslee', 'Grammar'),
(11, 'Write about the time when you were truly happy.', '', '', '', '','English', '', 'deslee', 'Composition'),
(12, 'The building of dams, bridges and roads,', 'eliminated food source', 'ruined living environment', 'changed breeding habits', 'minimised movement of animals','English', 'minimised movement of animals', 'deslee', 'Comprehension')
SET IDENTITY_INSERT Question OFF





CREATE TABLE Quiz (
QuizId INT IDENTITY (1, 1) PRIMARY KEY,
Title VARCHAR (150) NOT NULL,
Topic VARCHAR (50) NOT NULL,
Sec INT NOT NULL,
Dt DATETIME NOT NULL,
Duration INT NOT NULL,
UserCode VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Account(AccountId)
);

GO

SET IDENTITY_INSERT Quiz ON
INSERT INTO Quiz (QuizId, Title, Topic, Sec, Dt, Duration, UserCode) VALUES 
(1, 'PREPARATORY COURSE FOR ADMISSION TO GOVERNMENT SCHOOLS (Secondary 2-3)','English', 2, '2021-11-17 08:00', 130, 'deslee'),
(2, 'SEC 2 MONTHLY TEST PAPER 1', 'Mathematics', 2, '2021-11-13 08:00' , 35, 'deslee'),
(3, 'SEC 2 MONTHLY TEST PAPER 2', 'Mathematics', 2, '2021-10-08 08:00', 50, 'deslee'),
(4, 'PREPARATORY COURSE FOR ADMISSION TO GOVERNMENT SCHOOLS (Secondary 2-3)', 'English', 2, '2021-11-17 08:00', 130, 'deslee')
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

CREATE TABLE Teacher (
    TeacherId       INT  IDENTITY (1, 1) PRIMARY KEY,
    Name    VARCHAR (50) NOT NULL,
    MobileNo	VARCHAR (150) NOT NULL,
    Email     VARCHAR(100)    NOT NULL,
    Role    VARCHAR(100)         NOT NULL,
    AddedBy VARCHAR(50)         NOT NULL FOREIGN KEY REFERENCES Account(AccountId)
);

SET IDENTITY_INSERT Teacher ON
INSERT INTO Teacher (TeacherId, Name, MobileNo, Email, Role, AddedBy) VALUES 
('1','Max Lim', '66994455', 'MaxLim@genericemail.com', 'Part-Time', 'deslee'),
('2','Aloysius Lee', '11223344', 'AlLee@genericemail.com', 'Full-Time', 'deslee'),
('3','Russell Peters', '88990044', 'RussPete@genericemail.com', 'Full-Time', 'deslee'),
('4','Muhammad Zakariya', '22553366', 'MZakariya@genericemail.com', 'Part-Time', 'deslee')
SET IDENTITY_INSERT Teacher OFF

CREATE TABLE Student (
    StudentId       INT  IDENTITY (1, 1) PRIMARY KEY,
    Name    VARCHAR (50) NOT NULL,
    MobileNo	VARCHAR (150) NOT NULL,
    Country	VARCHAR (150) NOT NULL,
    Foreigner    BIT         NOT NULL,
    SchLvl	VARCHAR (150) NOT NULL,
    Email     VARCHAR(100)    NOT NULL,
    Class    VARCHAR(100)         NOT NULL,
    AddedBy VARCHAR(50)         NOT NULL FOREIGN KEY REFERENCES Account(AccountId)
);

SET IDENTITY_INSERT Student ON
INSERT INTO Student (StudentId, Name, MobileNo, Country, Foreigner, SchLvl, Email, Class, AddedBy) VALUES 
('1','Edna Chia', '66994455', 'Singapore', 0,'Sec 1', 'ednachia@genericemail.com', 'Class 1', 'deslee'),
('2','Lin Wei Jie', '11223344', 'China', 1, 'Sec 1', 'LinWJ123@genericemail.com', 'Class 1', 'deslee'),
('3','Muthu Shanmugam', '11223344', 'Singapore', 0, 'Sec 2', 'MShan123@genericemail.com', 'Class 2', 'deslee'),
('4','Muhammad Hashim', '11223344', 'Malaysia', 1, 'Sec 2', 'MHashim@genericemail.com', 'Class 2', 'deslee')
SET IDENTITY_INSERT Student OFF

CREATE TABLE Class (
    ClassId       INT  IDENTITY (1, 1) PRIMARY KEY,
    Name    VARCHAR (50) NOT NULL
);

SET IDENTITY_INSERT Class ON
INSERT INTO Class (ClassId, Name) VALUES 
('1','Class 1'),
('2','Class 2'),
('3','Class 3')
SET IDENTITY_INSERT Class OFF

CREATE TABLE TeacherClassBindDB (
    ID              INT  IDENTITY (1, 1) PRIMARY KEY,
	TeacherId       INT NOT NULL FOREIGN KEY REFERENCES Teacher(TeacherId),
    ClassId      INT NOT NULL FOREIGN KEY REFERENCES Class(ClassId)
);

SET IDENTITY_INSERT TeacherClassBindDB ON
INSERT INTO TeacherClassBindDB (ID, TeacherId, ClassId) VALUES 
(1,'1','1'),
(2, '1','2'),
(3, '1','3')
SET IDENTITY_INSERT TeacherClassBindDB OFF

CREATE TABLE StudentClassBindDB (
    ID              INT  IDENTITY (1, 1) PRIMARY KEY,
	StudentId       INT NOT NULL FOREIGN KEY REFERENCES Student(StudentId),
    ClassId      INT NOT NULL FOREIGN KEY REFERENCES Class(ClassId)
);

SET IDENTITY_INSERT StudentClassBindDB ON
INSERT INTO StudentClassBindDB (ID, StudentId, ClassId) VALUES 
(1,'1','1'),
(2, '2','1'),
(3, '3','1')
SET IDENTITY_INSERT StudentClassBindDB OFF

CREATE TABLE TeacherStudentBindDB (
    ID              INT  IDENTITY (1, 1) PRIMARY KEY,
	StudentId       INT NOT NULL FOREIGN KEY REFERENCES Student(StudentId),
    TeacherId      INT NOT NULL FOREIGN KEY REFERENCES Teacher(TeacherId)
);

SET IDENTITY_INSERT TeacherStudentBindDB ON
INSERT INTO TeacherStudentBindDB (ID, StudentId, TeacherId) VALUES 
(1,'1','1'),
(2, '1','2'),
(3, '1','3')
SET IDENTITY_INSERT TeacherStudentBindDB OFF

CREATE TABLE Announcement (
    Id INT IDENTITY (1, 1) PRIMARY KEY,
    ClassId INT NOT NULL FOREIGN KEY REFERENCES Class(ClassId),
    Remarks VARCHAR (1000) NOT NULL
);

SET IDENTITY_INSERT Announcement ON
INSERT INTO Announcement (Id, ClassId, Remarks) VALUES
(1, '1', 'All you fucking biatches'),
(2, '2', 'Class all of you are fuckfaces'),
(3, '3','this class is full of idiots'),
(4, '1', 'yall are motherfuckers, enough said. go and fuck your mom.')
SET IDENTITY_INSERT Announcement OFF








