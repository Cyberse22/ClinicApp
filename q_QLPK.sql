create database QLPK
go

use QLPK
go


-- UserRole ENUM ('admin', 'doctor', 'nurse', 'patient');
-- AppointmentStatus ENUM ('scheduled', 'confirmed', 'cancelled', 'completed');


-- Bang Tai khoan dang nhap
create table Account (
	id int identity primary key,
	username varchar(20) not null unique,
	password nvarchar(100) not null,
	avatar nvarchar(255),
	user_role varchar(10) check(user_role in ('admin', 'doctor', 'nurse', 'patient')) default 'patient',
	active bit not null default 1
);
go

-- Bang Thong tin nguoi dung
create table UserInfo (
	id int identity primary key,
	last_name nvarchar(50),
	first_name nvarchar(20),
	gender nvarchar(10),
	date_of_birth date,
	address nvarchar(150),
	phone varchar(12) unique,
	account_id int,
	active bit not null default 1

	foreign key (account_id) references Account(id)
);
go




-- Bang Admin
create table Admin (
	id int identity primary key,
	info_id int not null,
	active bit not null default 1,

	foreign key (info_id) references UserInfo(id),
);
go


-- Bang Bac si
create table Doctor (
	id int identity primary key,
	info_id int not null,
	specialty nvarchar(50),
	active bit not null default 1,

	foreign key (info_id) references UserInfo(id),
);
go


-- Bang Y ta
create table Nurse (
	id int identity primary key,
	info_id int not null,
	active bit not null default 1,

	foreign key (info_id) references UserInfo(id),
);
go 


-- Bang Benh nhan
create table Patient (
	id int identity primary key,
	info_id int not null,
	active bit not null default 1,

	foreign key (info_id) references UserInfo(id),
);
go


-- Benh Lich hen
create table Appointment (
	id int identity primary key,
	patient_id int,
	nurse_id int,
	doctor_id int,
	date date not null,
	time time not null,
	reason nvarchar(255),
	status varchar(10) check (status in ('scheduled', 'confirmed', 'cancelled', 'completed')) default 'scheduled',

	foreign key (patient_id) references Patient(id),
	foreign key (nurse_id) references Nurse(id),
	foreign key (doctor_id) references Doctor(id),
);
go


-- Bang Thuoc
create table Medicine (
	id int identity primary key,
	name nvarchar(100),
	unit_price decimal(10,2),
	active bit not null default 1
);
go


-- Bang Toa thuoc
create table Prescription (
	id int identity primary key,
	patient_id int,
	doctor_id int,
	appointment_id int,
	conclusion nvarchar(200),
	created_date datetime,

	foreign key (patient_id) references Patient(id),
	foreign key (doctor_id) references Doctor(id),
	foreign key (appointment_id) references Appointment(id)
);
go


-- Bang Chi tiet toa thuoc
create table Prescription_Detail (
	id int identity primary key,
	prescription_id int,
	medicine_id int,
	quantity int,

	foreign key (prescription_id) references Prescription(id),
	foreign key (medicine_id) references Medicine(id)
);
go


-- Bang Dich vu
create table Service (
	id int identity primary key,
	name nvarchar(100),
	unit_price decimal(10,2),
	active bit not null default 1
);
go


-- Bang Hoa don
create table Invoice (
	id int identity primary key,
	appointment_id int,
	prescription_id int,
	patient_id int,
	nurse_id int not null,
	total_amount decimal(10, 2),
	created_date datetime,

	foreign key (appointment_id) references Appointment(id),
	foreign key (prescription_id) references Prescription(id),
	foreign key (nurse_id) references Nurse(id)
);
go


-- Bang Chi tiet hoa don thuoc
create table Invoice_Medicine (
	id int identity primary key,
	invoice_id int,
	medicine_id int,
	quantity int,
	unit_price decimal(10,2),
	discount decimal(10,2),

	foreign key (invoice_id) references Invoice(id),
	foreign key (medicine_id) references Medicine(id)
);
go


-- Bang Chi tiet hoa don dich vu
create table Invoice_Service (
	id int identity primary key,
	invoice_id int,
	service_id int,
	quantity int,
	unit_price decimal(10,2),
	discount decimal(10,2),

	foreign key (invoice_id) references Invoice(id),
	foreign key (service_id) references Service(id)
);
go

-------------------------------------------------------------

use QLPK
go

drop table Invoice_Service
drop table Invoice_Medicine
drop table Invoice
drop table Service
drop table Prescription_Detail
drop table Prescription
drop table Medicine
drop table Appointment
drop table Patient
drop table Nurse
drop table Doctor
drop table Admin
drop table UserInfo
drop table Account

alter table Invoice
alter column created_date datetime

alter table Prescription
add conclusion nvarchar(200) 

alter table Prescription
add created_date datetime

-------------------------------------------------------------

use QLPK;
go

-- Insert into Account table
insert into Account (username, password, avatar, user_role, active, avatar)
values 
('admin1', 'matkhau123', null, 'admin', 1, 'https://www.w3schools.com/bootstrap5/img_avatar1.png'),
('doctor1', 'matkhau123', null, 'doctor', 1, 'https://www.w3schools.com/bootstrap5/img_avatar1.png'),
('nurse1', 'matkhau123', null, 'nurse', 1, 'https://www.w3schools.com/bootstrap5/img_avatar1.png'),
('benhnhan1', 'matkhau123', null, 'patient', 1, 'https://www.w3schools.com/bootstrap5/img_avatar1.png'),
('benhnhan2', 'matkhau123', null, 'patient', 1, 'https://www.w3schools.com/bootstrap5/img_avatar1.png');
go

-- Insert into UserInfo table
insert into UserInfo (last_name, first_name, gender, date_of_birth, address, phone, account_id, active)
values 
(N'Nguyễn', N'Quản Trị', N'Nam', '1980-01-01', N'123 Đường Chính', '0912345678', 1, 1),
(N'Lê', N'Bác Sĩ', N'Nam', '1975-02-02', N'456 Đường Phụ', '0912345679', 2, 1),
(N'Trần', N'Y Tá', N'Nữ', '1985-03-03', N'789 Đường Cây', '0912345680', 3, 1),
(N'Phạm', N'Bệnh Nhân1', N'Nữ', '1990-04-04', N'321 Đường Thông', '0912345681', 4, 1),
(N'Hoàng', N'Bệnh Nhân2', N'Nam', '1995-05-05', N'654 Đường Phong', '0912345682', 5, 1);
go

-- Insert into Admin table
insert into Admin (info_id, active)
values 
(1, 1);
go

-- Insert into Doctor table
insert into Doctor (info_id, specialty, active)
values 
(2, N'Chuyên Khoa Tim Mạch', 1);
go

-- Insert into Nurse table
insert into Nurse (info_id, active)
values 
(3, 1);
go

-- Insert into Patient table
insert into Patient (info_id, active)
values 
(4, 1),
(5, 1);
go

-- Insert into Appointment table
insert into Appointment (patient_id, nurse_id, doctor_id, date, time, reason, status)
values 
(1, 1, 1, '2024-05-20', '09:00', N'Khám tổng quát', 'scheduled'),
(2, 1, 1, '2024-05-21', '10:00', N'Tái khám', 'scheduled');
go

-- Insert into Medicine table
insert into Medicine (name, unit_price, active)
values 
(N'Paracetamol', 5000, 1),
(N'Ibuprofen', 10000, 1);
go

-- Insert into Prescription table
insert into Prescription (patient_id, doctor_id, appointment_id)
values 
(1, 1, 1),
(2, 1, 2);
go

-- Insert into Prescription_Detail table
insert into Prescription_Detail (prescription_id, medicine_id, quantity)
values 
(1, 1, 2),
(1, 2, 1),
(2, 1, 1),
(2, 2, 2);
go

-- Insert into Service table
insert into Service (name, unit_price, active)
values 
(N'Xét nghiệm máu', 150000, 1),
(N'Chụp X-quang', 200000, 1);
go

-- Insert into Invoice table
insert into Invoice (appointment_id, prescription_id, patient_id, nurse_id, total_amount, created_date)
values 
(1, 1, 1, 1, 500000, "2024-05-20 10:00:00.000"),
(2, 2, 2, 1, 600000, "2024-05-21 11:00:00.000");
go

-- Insert into Invoice_Medicine table
insert into Invoice_Medicine (invoice_id, medicine_id, quantity, unit_price, discount)
values 
(1, 1, 2, 5000, 0),
(1, 2, 1, 10000, 0),
(2, 1, 1, 5000, 0),
(2, 2, 2, 10000, 0);
go

-- Insert into Invoice_Service table
insert into Invoice_Service (invoice_id, service_id, quantity, unit_price, discount)
values 
(1, 1, 1, 150000, 0),
(1, 2, 1, 200000, 0),
(2, 1, 1, 150000, 0),
(2, 2, 1, 200000, 0);
go
