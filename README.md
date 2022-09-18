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

### v3

