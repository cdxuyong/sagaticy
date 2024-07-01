#chr management 

## versions

### v1

publish 2014~2019

### version 2.1

publish: 2020/3/28  

modifys:  

1. add UseType
2. import employee from excel

### version 2.9.1

add hr.employee.tuiyijunren

### version 2.9.2

em hr

### version 2.9.3

update hr.employee more fields 4 wt

## 记录

```sql
alter table HR_EMPLOYEE add UseType int;
UPDATE HR_EMPLOYEE set UseType=0;
```

### version 2.7.0

```sql
alter table HR_SALARY_PAY_OBJECT add memo varchar(255);
insert into HR_SALARY_ITEM(item_id,PARENT_ID,ITEM_TYPE,ITEM_NAME,ITEM_TITLE,ISLEAF,ITEM_CODE,STATE) values(208,2,2,'个人应扣.意外险','意外险',1,'002-008',0);
insert into HR_SALARY_TEMPLATE_ITEM(TEMPLATE_ID,ITEM_ID,ITEM_EIDTABLE) values(1,208,0);
```

### version 2.3.0

```sql
 insert T_S_ORGANIZATION(ORG_ID,PARENT_ID,ORG_TYPE,ORG_NAME,ORG_CODE,STATE) values(100000,0,2,'其他单位','002',0);

alter table T_S_USER add COMPANYID int;
UPDATE T_S_USER set COMPANYID=0;
```

### version 2.3.1

```sql

-- 数据导入模型调整
alter table HR_SALARY_IMPORTOR_DATA add BASE_VALUE numeric(18, 2);
alter table HR_SALARY_IMPORTOR_DATA add SCALE_PERSON numeric(18, 2);
alter table HR_SALARY_IMPORTOR_DATA add SCALE_CMP numeric(18, 2);
alter table HR_SALARY_IMPORTOR_DATA add PAY_INDEX varchar(16);
alter table HR_SALARY_IMPORTOR_DATA add ACCOUNT_INDEX varchar(16);
alter table HR_SALARY_IMPORTOR_DATA add MEMO varchar(256);


```