﻿//
//  JsonArray.cs
//
//  Author:
//       田小宁 <springrain1991@hotmail.com>
//
//  Copyright (c) 2017 田小宁
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Collections;
using System;
namespace CsharpJson
{
    /// <summary>
    /// Json array.
    /// Json的Array型数据，其元素是列表型
    /// </summary>
    public sealed class JsonArray : BaseType, IEnumerable
    {
        /// <summary>
        /// The arrylist.
        /// 保存列表型数据
        /// </summary>
        private List<JsonValue> arrylist;
        /// <summary>
        /// Initializes a new instance of the<see cref="CsharpJson.JsonArray"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonArray"/>类实例
        /// </summary>
        public JsonArray()
        {
            arrylist = new List<JsonValue>();
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="CsharpJson.JsonArray"/> class.
		/// 用指定的int[]初始化一个新的<see cref="CsharpJson.JsonArray"/>类的实例;
		/// 异常：当values为null时引发异常
		/// </summary>
		/// <param name="values">Values.</param>
		public JsonArray(int []values)
		{
			arrylist = new List<JsonValue> ();
			if (values==null)
			{
				throw new ArgumentNullException ();
			}
			for (int i = 0; i < values.Length; ++i)
			{
				this.arrylist.Add(values[i]);
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CsharpJson.JsonArray"/> class.
		/// 用指定的double[]初始化一个新的<see cref="CsharpJson.JsonArray"/>类的实例;
		/// 异常：当values为null时引发异常
		/// </summary>
		/// <param name="values">Values.</param>
		public JsonArray(double []values)
		{
			arrylist = new List<JsonValue> ();
			if(values==null)
			{
				throw new ArgumentNullException ();
			}
			for(int i=0;i<values.Length;++i)
			{
				this.arrylist.Add (values[i]);
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CsharpJson.JsonArray"/> class.
		/// 用指定的string[]初始化一个新的 <see cref="CsharpJson.JsonArray"/>类的实例;
		/// 异常：当values为null时引发异常
		/// </summary>
		/// <param name="values">Values.</param>
		public JsonArray(string []values)
		{
			arrylist = new List<JsonValue> ();
			if(values==null)
			{
				throw new ArgumentNullException ();
			}
			for(int i=0;i<values.Length;++i)
			{
				this.arrylist.Add (values[i]);
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CsharpJson.JsonArray"/> class.
		/// 用指定的List<string>初始化一个新的<see cref="CsharpJson.JsonArray"/>类的实例;
		/// 异常：当strlist为null时引发异常
		/// </summary>
		/// <param name="strlist">Strlist.</param>
		public JsonArray(List<string>strlist)
		{
			arrylist = new List<JsonValue> ();
			if (strlist == null)
			{
				throw new ArgumentNullException ();
			}
			foreach (string iter in strlist)
			{
				this.arrylist.Add(new JsonValue(iter));
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonArray"/> class.
		/// 初始化一个新的 <see cref="CsharpJson.JsonArray"/>类的实例;
		/// 异常：当arr为null时引发异常
        /// </summary>
        /// <param name="arr">Arr.</param>
        public JsonArray(JsonArray arr)
        {
			if(arr==null)
			{
				throw new ArgumentNullException ();
			}
            for (int i = 0; i < arr.Count; ++i)
            {
                this.arrylist.Add(arr[i]);
            }
        }
        /// <summary>
        /// Gets the count.
        /// 获取元素数量
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return this.arrylist.Count; }
        }
        /// <summary>
        /// Gets or sets the <see cref="CsharpJson.JsonArray"/> with the specified i.
        /// 获取或设置指定索引处的<see cref="CsharpJson.JsonArray"/>类的实例
        /// </summary>
        /// <param name="i">The index.</param>
        public JsonValue this[int i]
        {
            get
            {
            	return this.arrylist[i];
            }
            set
            {
                if (value == null)
                {
                    this.arrylist.Add(new JsonValue());
                }
                else
                {
                    this.arrylist.Add(value);
                }
            }
        }
        /// <summary>
        /// Add the specified values.
        /// 添加指定的int数组
        /// </summary>
        /// <param name="values">Values.</param>
        public void Adds(int[] values)
        {
            if (values==null)
            {
				throw new ArgumentNullException ();
            }
            for (int i = 0; i < values.Length; ++i)
            {
                this.arrylist.Add(values[i]);
            }
        }

        /// <summary>
        /// Add the specified values.
        /// 添加指定的double数组
        /// </summary>
        /// <param name="values">Values.</param>
        public void Adds(double[] values)
        {
            if (values == null)
            {
				throw new ArgumentNullException ();
            }
            for (int i = 0; i < values.Length; ++i)
            {
                this.arrylist.Add(values[i]);
            }
        }
        /// <summary>
        /// Add the specified values.
        /// 添加指定的string数组
        /// </summary>
        /// <param name="values">Values.</param>
        public void Adds(string[] values)
        {
            if (values == null)
            {
				throw new ArgumentNullException ();
            }
            for (int i = 0; i < values.Length; ++i)
            {
                this.arrylist.Add(new JsonValue(values[i]));
            }
        }
        /// <summary>
        /// Add the specified strlist.
        /// 添加指定的string类型List
        /// </summary>
        /// <param name="strlist">Strlist.</param>
        public void Adds(List<string> strlist)
        {
            if (strlist == null)
            {
				throw new ArgumentNullException ();
            }
            foreach (string iter in strlist)
            {
                this.arrylist.Add(new JsonValue(iter));
            }
        }
        /// <summary>
        /// Add the JsonValue value.
        /// 添加JsonValue值
        /// </summary>
        /// <param name="value">Value.</param>
        public void Add(JsonValue value)
        {
			if (value == null) 
			{
				this.arrylist.Add (new JsonValue ());
			} else
			{
				this.arrylist.Add (value);
			}
        }
        /// <summary>
        /// if Contains the JsonValue item return true otherwise false.
        /// 如果包含指定的JsonValue类型的元素，则返回true其他情况返回false
        /// </summary>
        /// <param name="item">Item.</param>
        public bool Contains(JsonValue item)
        {
            return this.arrylist.Contains(item);
        }
        /// <summary>
        /// Gets the enumerator.
        /// 获取计数器（迭代）
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.arrylist.GetEnumerator();
        }
		public JsonValue value(int index)
		{
			return this.arrylist[index];
		}
    }
}

