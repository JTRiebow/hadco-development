using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Hadco.Web.Models
{
    /*
     * NOTE: private readonly constants are defined at the bottom of this file. All this that should be edited, 
     * or copied for new definitions will be found near the top of this file.
     */

    /// <summary>
    ///     Used as attributes on endpoints in order for special Swagger parameters to show up.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EndpointQueryStrings : Attribute
    {

        /// <summary>
        ///     Set to true in order to have the ordering parameter show up in Swagger documentation
        /// </summary>
        public bool Ordering = true;
        private List<Parameter> GetOrderingParams()
        {
            List<Parameter> paramList = new List<Parameter>();
	        if (Ordering)
	        {
		        Parameter orderingParam = new Parameter();
		        orderingParam.description = "Sort the result set by one or more fields specified in orderby. " +
		                                    "Sort direction can be ASC or DESC. Default is Ascending.";
		        DataType type = TYPE_STRING;
		        orderingParam.@in = IN_FIELD_QUERY;
		        orderingParam.format = type.format;
		        orderingParam.type = type.type;
		        orderingParam.name = "orderby";
		        orderingParam.required = false;

		        paramList.Add(orderingParam);
	        }
	        return paramList;
        }

        /// <summary>
        ///     Set to true in order to have the pagination parameter show up in Swagger documentation
        /// </summary>
        public bool Pagination = true;
        private List<Parameter> GetPaginationParams()
        {
            List<Parameter> paramList = new List<Parameter>();
	        if (Pagination)
	        {
		        Parameter takeParam = new Parameter();
		        DataType typeTake = TYPE_INTEGER;
		        takeParam.@in = IN_FIELD_QUERY;
		        takeParam.format = typeTake.format;
		        takeParam.type = typeTake.type;
		        takeParam.name = "take";
		        takeParam.description = "Returns number of records specified by take value.";
		        takeParam.required = false;

		        Parameter skipParam = new Parameter();
		        DataType typeSkip = TYPE_INTEGER;
		        skipParam.@in = IN_FIELD_QUERY;
		        skipParam.format = typeSkip.format;
		        skipParam.type = typeSkip.type;
		        skipParam.name = "skip";
		        skipParam.description = "Skip number of records specified by skip value.";
		        skipParam.required = false;

		        paramList.Add(takeParam);
		        paramList.Add(skipParam);
	        }
	        return paramList;
        }

        /// <summary>
        ///     Set to true in order to have the filtering parameter show up in Swagger documentation
        /// </summary>
        public bool Filtering = true;
        private List<Parameter> GetFilteringParams()
        {
            List<Parameter> paramList = new List<Parameter>();
	        if (Filtering)
	        {
		        Parameter filterParam = new Parameter();
		        DataType type = TYPE_STRING;
		        filterParam.@in = IN_FIELD_QUERY;
		        filterParam.format = type.format;
		        filterParam.type = type.type;
		        filterParam.name = "filter";
		        filterParam.description = "";
		        filterParam.required = false;

		        paramList.Add(filterParam);
	        }
	        return paramList;
        }

        /// <summary>
        ///     Set to true in order to have the search fields parameters show up in Swagger documentation
        /// </summary>
        public bool SearchFields = true;
        private List<Parameter> GetSearchFieldsParams()
        {
            List<Parameter> paramList = new List<Parameter>();
            if (SearchFields)
            {
                Parameter searchFieldsParam = new Parameter();
                DataType typeTake = TYPE_STRING;
                searchFieldsParam.@in = IN_FIELD_QUERY;
                searchFieldsParam.format = typeTake.format;
                searchFieldsParam.type = typeTake.type;
                searchFieldsParam.name = "searchFields";
                searchFieldsParam.description = "A comma sepearated list of fields to search. Must be used in conjunction with the 'search' parameter.";
                searchFieldsParam.required = false;

                Parameter searchParam = new Parameter();
                DataType typeSkip = TYPE_INTEGER;
                searchParam.@in = IN_FIELD_QUERY;
                searchParam.format = typeSkip.format;
                searchParam.type = typeSkip.type;
                searchParam.name = "search";
                searchParam.description = "A space delimited list of terms to search on. Must be used in conjunction with the 'searchFields' parameter.";
                searchParam.required = false;

                paramList.Add(searchFieldsParam);
                paramList.Add(searchParam);
            }
            return paramList;
        }
        /*
        * ========================================================================================================
        *	Everything found below here are items that shouldn't need to be modified very often, if ever.
        * ======================================================================================================== 
        */

        /// <summary>
        ///     Returns the Swagger parameter information for use in the swagger documentation.
        /// </summary>
        /// <returns>The parameters to display in swagger.</returns>
        public IList<Parameter> GetSwaggerParams()
        {
            List<Parameter> parameters = null;

            FieldInfo[] fields = this.GetType().GetFields();
            foreach (FieldInfo f in fields)
            {
                if (parameters == null)
                {
                    parameters = new List<Parameter>();
                }

                MethodInfo paramMethod = this.GetType().GetMethod(METHOD_PREFIX + f.Name + METHOD_POSTFIX,
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (paramMethod != null)
                {
                    List<Parameter> paramList = (List<Parameter>)paramMethod.Invoke(this, null);
                    foreach (Parameter param in paramList)
                    {
                        if (parameters.Any(c => c.name == param.name))
                        {
                            throw new CustomAttributeFormatException(String.Format("Duplicate query param ({0}) found" +
                                "when building documentation from custom attributes.", param.name));
                        }
                        parameters.Add(param);
                    }
                }
            }

            return parameters;
        }

        private const string METHOD_PREFIX = "Get";
        private const string METHOD_POSTFIX = "Params";

        private const string IN_FIELD_QUERY = "query";
        private const string IN_FIELD_HEADER = "header";
        private const string IN_FIELD_PATH = "path";
        private const string IN_FIELD_FORM_DATA = "formData";
        private const string IN_FIELD_BODY = "body";

        private readonly DataType TYPE_INTEGER = new DataType("integer", "int32");
        private readonly DataType TYPE_LONG = new DataType("integer", "int64");
        private readonly DataType TYPE_FLOAT = new DataType("number", "float");
        private readonly DataType TYPE_DOUBLE = new DataType("number", "double");
        private readonly DataType TYPE_STRING = new DataType("string", "string");
        private readonly DataType TYPE_BYTE = new DataType("string", "byte");
        private readonly DataType TYPE_BOOLEAN = new DataType("boolean", "boolean");
        private readonly DataType TYPE_DATE = new DataType("string", "date");
        private readonly DataType TYPE_DATE_TIME = new DataType("string", "date-time");

        class DataType
        {
            public string type;
            public string format;

            public DataType(string type, string format)
            {
                this.type = type;
                this.format = format;
            }
        }
    }
}