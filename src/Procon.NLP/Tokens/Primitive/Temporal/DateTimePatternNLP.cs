﻿using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal {

    [Serializable]
    public class DateTimePatternNlp {

        /// <summary>
        /// If the date/time should be interpretted as a definte date/time that specifies a moment
        /// or if it should be used as a relative offset from right now.
        /// </summary>
        public TimeType Rule { get; set; }

        /// <summary>
        /// Specifies how the time is used, as a delay from now, an interval or specifying a period of time.
        /// </summary>
        public TimeModifier Modifier { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public System.DayOfWeek? DayOfWeek { get; set; }

        public int? Hour { get; set; }

        public int? Minute { get; set; }

        public int? Second { get; set; }

        /// <summary>
        /// First Monday in August { DayOfWeek = Monday, Interval = First, Month = 8 (August) }
        /// </summary>
        public TemporalInterval TemporalInterval { get; set; }

        public DateTimePatternNlp() {
            this.TemporalInterval = TemporalInterval.Infinite;
        }

        /// <summary>
        /// Fetches a copy of DateTimePatternNLP with the current time or the parsed time.
        /// </summary>
        /// <param name="now">Optional, if null the current date/time will be used.</param>
        /// <returns></returns>
        public static DateTimePatternNlp Now(DateTime? now = null) {
            if (now == null) {
                now = DateTime.Now;
            }

            return new DateTimePatternNlp() {
                Year = now.Value.Year,
                Month = now.Value.Month,
                Day = now.Value.Day,
                Hour = now.Value.Hour,
                Minute = now.Value.Minute,
                Second = now.Value.Second
            };
        }

        /// <summary>
        /// Combines two DateTimePatternNLP's, adding together elements if we already have them.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        private static object Combine(object o1, object o2) {

            object returnObject = null;

            if (o1 != null) {
                returnObject = o1;

                if (o2 != null && o1 is int && o2 is int) {
                    returnObject = (int)o1 + (int)o2;
                }
                else if (o2 != null && o1 is TemporalInterval && o2 is TemporalInterval) {

                    if (((TemporalInterval)o1) == TemporalInterval.Infinite && ((TemporalInterval)o2) != TemporalInterval.Infinite) {
                        returnObject = o2;
                    }
                }
            }
            else if (o2 != null) {
                returnObject = o2;
            }

            return returnObject;
        }

        private static object AnchoredCombine(object anchor, object defaultAnchor, object relative, object defaultRelative) {
            object returnObject = null;

            object usedAnchor = anchor ?? defaultAnchor;
            object usedRelative = relative ?? defaultRelative;

            if (usedAnchor is int && usedRelative is int) {
                returnObject = (int)usedAnchor + (int)usedRelative;
            }

            return returnObject;
        }

        protected static DateTimePatternNlp AddDefinitives(DateTimePatternNlp definitive1, DateTimePatternNlp definitive2) {
            return new DateTimePatternNlp {
                Rule = TimeType.Definitive,
                Year = definitive1.Year ?? definitive2.Year,
                Month = definitive1.Month ?? definitive2.Month,
                Day = definitive1.Day ?? definitive2.Day,
                Hour = definitive1.Hour ?? definitive2.Hour,
                Minute = definitive1.Minute ?? definitive2.Minute,
                Second = definitive1.Second ?? definitive2.Second,
                DayOfWeek = definitive1.DayOfWeek ?? definitive2.DayOfWeek,
                TemporalInterval = definitive1.TemporalInterval != TemporalInterval.Infinite ? definitive1.TemporalInterval : definitive2.TemporalInterval
            };
        }

        protected static DateTimePatternNlp AddDefinitiveRelative(DateTimePatternNlp definitive, DateTimePatternNlp relative) {
            return new DateTimePatternNlp {
                Rule = TimeType.Definitive,
                Year = (int?) DateTimePatternNlp.AnchoredCombine(definitive.Year, DateTime.Now.Year, relative.Year, 0),
                Month = (int?) DateTimePatternNlp.AnchoredCombine(definitive.Month, DateTime.Now.Month, relative.Month, 0),
                Day = (int?) DateTimePatternNlp.AnchoredCombine(definitive.Day, DateTime.Now.Day, relative.Day, 0),
                Hour = (int?) DateTimePatternNlp.AnchoredCombine(definitive.Hour, DateTime.Now.Hour, relative.Hour, 0),
                Minute = (int?) DateTimePatternNlp.AnchoredCombine(definitive.Minute, DateTime.Now.Minute, relative.Minute, 0),
                Second = (int?) DateTimePatternNlp.AnchoredCombine(definitive.Second, DateTime.Now.Second, relative.Second, 0),
                DayOfWeek = (System.DayOfWeek?) DateTimePatternNlp.AnchoredCombine(definitive.DayOfWeek, DateTime.Now.DayOfWeek, relative.DayOfWeek, 0),
                TemporalInterval = (TemporalInterval) DateTimePatternNlp.Combine(definitive.TemporalInterval, relative.TemporalInterval)
            };
        }

        protected static DateTimePatternNlp AddRelatives(DateTimePatternNlp relative1, DateTimePatternNlp relative2) {
            return new DateTimePatternNlp {
                Rule = TimeType.Relative,
                Year = (int?) DateTimePatternNlp.Combine(relative1.Year, relative2.Year),
                Month = (int?) DateTimePatternNlp.Combine(relative1.Month, relative2.Month),
                Day = (int?) DateTimePatternNlp.Combine(relative1.Day, relative2.Day),
                Hour = (int?) DateTimePatternNlp.Combine(relative1.Hour, relative2.Hour),
                Minute = (int?) DateTimePatternNlp.Combine(relative1.Minute, relative2.Minute),
                Second = (int?) DateTimePatternNlp.Combine(relative1.Second, relative2.Second),
                DayOfWeek = (System.DayOfWeek?) DateTimePatternNlp.Combine(relative1.DayOfWeek, relative2.DayOfWeek),
                TemporalInterval = (TemporalInterval) DateTimePatternNlp.Combine(relative1.TemporalInterval, relative2.TemporalInterval)
            };
        }

        public static DateTimePatternNlp operator +(DateTimePatternNlp one, DateTimePatternNlp two) {
            DateTimePatternNlp newDateTimePattern = null;

            if (one.Modifier == two.Modifier || one.Modifier == TimeModifier.None || two.Modifier == TimeModifier.None) {
                if (one.Rule == TimeType.Definitive && two.Rule == TimeType.Definitive) {
                    newDateTimePattern = DateTimePatternNlp.AddDefinitives(one, two);
                }
                else if (one.Rule == TimeType.Relative && two.Rule == TimeType.Definitive) {
                    newDateTimePattern = DateTimePatternNlp.AddDefinitiveRelative(two, one);
                }
                else if (one.Rule == TimeType.Definitive && two.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNlp.AddDefinitiveRelative(one, two);
                }
                else if (one.Rule == TimeType.Relative && two.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNlp.AddRelatives(one, two);
                }

                if (newDateTimePattern != null) {
                    newDateTimePattern.Modifier = one.Modifier == TimeModifier.None ? two.Modifier : one.Modifier;
                }
            }

            return newDateTimePattern;
        }

        public DateTimePatternNlp Negate() {

            DateTimePatternNlp newDateTimePattern = new DateTimePatternNlp();

            if (this.Year != null) newDateTimePattern.Year = this.Year * -1;
            if (this.Month != null) newDateTimePattern.Month = this.Month * -1;
            if (this.Day != null) newDateTimePattern.Day = this.Day * -1;
            if (this.Hour != null) newDateTimePattern.Hour = this.Hour * -1;
            if (this.Minute != null) newDateTimePattern.Minute = this.Minute * -1;
            if (this.Second != null) newDateTimePattern.Second = this.Second * -1;

            return newDateTimePattern;
        }

        public static DateTimePatternNlp operator -(DateTimePatternNlp one, DateTimePatternNlp two) {
            DateTimePatternNlp newDateTimePattern = null;

            if (one.Modifier == two.Modifier || one.Modifier == TimeModifier.None || two.Modifier == TimeModifier.None) {
                //if (c1.Rule == TimeType.Definitive && c2.Rule == TimeType.Definitive) {
                //    newDateTimePattern = DateTimePatternNLP.AddDefinitives(c1, c2);
                //}
                if (one.Rule == TimeType.Relative && two.Rule == TimeType.Definitive) {
                    newDateTimePattern = DateTimePatternNlp.AddDefinitiveRelative(two.Negate(), one);
                }
                else if (one.Rule == TimeType.Definitive && two.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNlp.AddDefinitiveRelative(one, two.Negate());
                }
                else if (one.Rule == TimeType.Relative && two.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNlp.AddRelatives(one, two.Negate());
                }

                if (newDateTimePattern != null) {
                    newDateTimePattern.Modifier = one.Modifier == TimeModifier.None ? two.Modifier : one.Modifier;
                }
            }

            return newDateTimePattern;
        }

        public TimeSpan? ToTimeSpan() {
            TimeSpan? ts = null;

            if (this.Modifier == TimeModifier.Interval) {
                ts = new TimeSpan(
                    this.Day == null ? 0 : (int)this.Day,
                    this.Hour == null ? 0 : (int)this.Hour,
                    this.Minute == null ? 0 : (int)this.Minute,
                    this.Second == null ? 0 : (int)this.Second
                );
            }
            else {
                DateTime now = DateTime.Now;
                DateTime? thisDateTime = this.ToDateTime(now);

                if (thisDateTime.HasValue == true) {
                    TimeSpan tots = thisDateTime.Value - now;

                    ts = new TimeSpan((long)Math.Ceiling(tots.Ticks / (double)TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
                }
            }

            return ts;
        }

        public DateTime? ToDateTime(DateTime? now = null) {
            DateTimePatternNlp dtp = this;

            if (now == null) {
                now = DateTime.Now;
            }

            if (this.Rule == TimeType.Relative) {
                dtp = DateTimePatternNlp.AddDefinitiveRelative(DateTimePatternNlp.Now(now), this);
            }

            DateTime dt = new DateTime(
                1,
                1,
                1,
                0,
                0,
                0
                );

            dt = dt.AddYears((dtp.Year != null ? (int)dtp.Year : now.Value.Year) - 1);
            dt = dt.AddMonths((dtp.Month != null ? (int)dtp.Month : now.Value.Month) - 1);
            dt = dt.AddDays((dtp.Day != null ? (int)dtp.Day : now.Value.Day) - 1);
            dt = dt.AddHours(dtp.Hour != null ? (int)dtp.Hour : now.Value.Hour);
            dt = dt.AddMinutes(dtp.Minute != null ? (int)dtp.Minute : now.Value.Minute);
            dt = dt.AddSeconds(dtp.Second != null ? (int)dtp.Second : now.Value.Second);

            return dt;
        }

        public override string ToString() {
            string rs = String.Empty;

            if (this.Modifier == TimeModifier.Interval) {
                rs = "Every " + this.ToTimeSpan();
            }
            else {
                rs = this.ToDateTime().ToString();
            }

            return rs;
        }
    }
}
