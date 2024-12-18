﻿using Dapper;
using System.Data;

namespace API.Tools {
  public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly> {
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value) {
      parameter.DbType = DbType.Date;
      parameter.Value = value;
    }
  }

  public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly> {
    public override TimeOnly Parse(object value) => TimeOnly.FromTimeSpan((TimeSpan)value);

    public override void SetValue(IDbDataParameter parameter, TimeOnly value) {
      parameter.DbType = DbType.Time;
      parameter.Value = value;
    }
  }
}
