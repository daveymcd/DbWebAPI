using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Helpers
{
    /// <summary> General Helper Class </summary>
    public static class Helpers
    {
        /// <summary>
        ///     DbWebApi.Helpers.DropListUBD
        ///     Class for Drop Down List key pairings.
        /// </summary>
        // Drop Down List Helpers
        public class DropListItem
        {
            ///<summary>Document Type Id</summary>
            public object Id { get; set; }
            ///<summary>Description of Document Type</summary>
            public string Text { get; set; }
        }
        /// <summary>
        ///     DbWebApi.Helpers.DropListUBD
        ///     Drop Down List for Use-By-Date Indicator (not-applicable/OK/Out-Of-Date)
        /// </summary>
        /// <example>
        ///     @Html.DropDownListFor(model => model.sCxItem.CheckUBD, new SelectList(Helpers.DropListUBD, "id", "text"))
        /// </example>
        public static List<DropListItem> DropListUBD = new()
        {
            //new DropListItem { id = null, text = "" }, // position zero in list is not assigned!
            new DropListItem { Id = 0, Text = "\u274E n/a"},
            new DropListItem { Id = 1, Text = "\u2705 Checked"},
            new DropListItem { Id = 2, Text = "\u26D4 Expired"}
        };
        /// <summary>
        ///     DbWebApi.Helpers.DropListUBD
        ///     Drop Down List for Document Type (SC1: - SC9:)
        /// </summary>
        /// <example>
        ///     @Html.DropDownListFor(model => model.sCxItem.Type, new SelectList(Helpers.DropListSCx, "id", "text"))
        /// </example>
        public static List<DropListItem> DropListSCx = new()
        {
            new DropListItem { Id = "",     Text = ""}, // position zero in list is not assigned!
            new DropListItem { Id = "SC1:", Text = "SC1: Deliveries-In"},
            new DropListItem { Id = "SC2:", Text = "SC2: Chiller Checks"},
            new DropListItem { Id = "SC3:", Text = "SC3: Cooking Log"},
            new DropListItem { Id = "SC4:", Text = "SC4: Hot Holding"},
            new DropListItem { Id = "SC5:", Text = "SC5: Hygiene Inspection"},
            new DropListItem { Id = "SC6:", Text = "SC6: Hygiene Training"},
            new DropListItem { Id = "SC7:", Text = "SC7: Fitness To Work"},
            new DropListItem { Id = "SC8:", Text = "SC8: All-In-One Form (SC1: - SC4: inc)"},
            new DropListItem { Id = "SC9:", Text = "SC9: Deliveries-Out"},
            new DropListItem { Id = "OPN:", Text = "OPN: Opening Checks" },
            new DropListItem { Id = "CLS:", Text = "CLS: Closing Checks"}
        };
        /// <summary>
        ///     DbWebApi.Helpers.GetReflectedPropertyValue(object, string)
        ///     Get the value of an object property from a complex structure
        /// </summary>
        /// <example>
        ///     var reflectedValue = Helpers.GetReflectedPropertyValue(sCxItem, "TimeStamp");
        /// </example>
        /// <param name="_object">structure</param>
        /// <param name="_property">Property to get</param>
        public static string GetReflectedPropertyValue(this object _object, string _property)
        {
            object reflectedValue = _object.GetType().GetProperty(_property).GetValue(_object, null);
            return reflectedValue != null ? reflectedValue.ToString() : "";
        }
    }
}
