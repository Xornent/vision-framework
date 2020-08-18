using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;

namespace Vision.Utilities {

    public static class Query {

        public static (bool Success, Vision.Models.Page Result) GetPageByMd5
            (Vision.Data.PageContext ctx, string md5) {

            var query = from page in ctx.Page
                        where page.Hash == md5
                        select page;
            var result = query.FirstOrDefault();
            if (result == null) return (false, null);
            return (true, result);
        }

        public static (bool Success, Vision.Models.Page Result, Vision.Models.Record Record) GetPageAndRecordByMd5
            (Vision.Data.PageContext ctx, Vision.Data.RecordContext ctx_record, string md5) {

            var query = from page in ctx.Page
                        where page.Hash == md5
                        select page;
            var result = query.FirstOrDefault();
            if (result == null) return (false, null, null);

            var q_record = from rec in ctx_record.Record
                           where rec.Id == result.Id
                           select rec;
            var result_rec = q_record.FirstOrDefault();
            return (true, result, result_rec);
        }

        public static (bool Success, Vision.Models.Page Result) GetPageById
            (Vision.Data.PageContext ctx, int id) {

            var query = from page in ctx.Page
                        where page.Id == id
                        select page;
            var result = query.FirstOrDefault();
            if (result == null) return (false, null);
            return (true, result);
        }

        public static (bool Success, Vision.Models.Page Result, Vision.Models.Record Record) GetPageAndRecordById
            (Vision.Data.PageContext ctx, Vision.Data.RecordContext ctx_record, int id) {

            var query = from page in ctx.Page
                        where page.Id == id
                        select page;
            var result = query.FirstOrDefault();
            if (result == null) return (false, null, null);

            var q_record = from rec in ctx_record.Record
                           where rec.Id == result.Id
                           select rec;
            var result_rec = q_record.FirstOrDefault();
            return (true, result, result_rec);
        }

        public static User GetUserById(UserContext ctx, int id) {
            var query = from user in ctx.User
                        where user.Id == id
                        select user;
            var result = query.FirstOrDefault();
            return result;
        }

        public static User GetUserByName(UserContext ctx, string name) {
            var query = from user in ctx.User
                        where user.Display == name
                        select user;
            var result = query.FirstOrDefault();
            return result;
        }

        public static Category GetCategoryById(CategoryContext ctx, int id) {
            var query = from user in ctx.Category
                        where user.Id == id
                        select user;
            var result = query.FirstOrDefault();
            return result;
        }

        public static Category GetCategoryByName(CategoryContext ctx, string name) {
            var query = from user in ctx.Category
                        where user.Name.ToLower() == name.ToLower()
                        select user;
            var result = query.FirstOrDefault();
            return result;
        }
    }
}
