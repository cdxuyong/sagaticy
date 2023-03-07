#chr management 

## versions

### v1

publish 2014~2019

### version 2.1

publish: 2020/3/28  

modifys:  

1. 增加UseType字段，区分发放和档案统计
2. 人员管理批量导入功能

```sql
alter table HR_EMPLOYEE add UseType int;
UPDATE HR_EMPLOYEE set UseType=0;
```

### version 2.3.0

```sql
 insert T_S_ORGANIZATION(ORG_ID,PARENT_ID,ORG_TYPE,ORG_NAME,ORG_CODE,STATE) values(100000,0,2,'其他单位','002',0);

alter table T_S_USER add COMPANYID int;
UPDATE T_S_USER set COMPANYID=0;
```