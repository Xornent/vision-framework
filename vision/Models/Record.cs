using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models {

    public class Record {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所有历史版本的差异报告集合
        /// 
        /// 我们将所有的内容（从初始 0 版本开始）的差异全部保存在多行的 History 字符串中。 类似 Git
        /// 差异，我们已单行的命令记录增加和删除的行。 每个不同的历史记录之间是使用 @ 标记的附加信息，
        /// 包括更改时间，更改作者，历史记录编码和修改描述。 每一个历史记录描述的格式如下
        /// 
        /// :1
        /// @user xornent
        /// @datetime 2020/07/24 14:39:00
        /// @summary the summary text.  \n    indent 4 \nmultiline...
        /// 
        /// 历史记录描述之后，是描述增减位置的语句。 命令只有两个，分别是插入（INS）和删除（DEL），
        /// 每个命令传入一个整形参数指示插入和删除的行。 这两个命令被简写成 + 和 -。 下面的例子是在
        /// 一个空文档中插入的文本
        /// 
        /// (INS 0)第一行的内容
        /// (INS 1)第二行的内容
        /// (INS 2)第三行的内容
        /// 
        /// 对于有内容的文档，生成的当行 Git 差异报告如下：
        /// 
        /// 源文档：
        ///   1 public string History { get; set; }
        ///   2 public int Use { get; set; }
        ///   3 
        ///   4 public void Save() {
        /// 
        /// 目标文档：
        ///   1 public string History { get; set; }
        ///   2 public int Use { get; private set; }
        ///   3 
        ///   4 [Obsolete]
        ///   5 public void Save() {
        ///   
        /// Git Inline Diff：
        ///     public string History { get; set; }
        ///   - public int Use { get; set; }
        ///   + public int Use { get; private set; }
        ///     
        ///   + [Obsolete]
        ///     public void Save() {
        ///   
        /// History Diff：
        ///     :2
        ///     @user _______
        ///     @datetime _______
        ///     @summary 
        ///     (- 1)
        ///     (+ 1)public int Use { get; private set; }
        ///     (+ 3)[Obsolete]
        ///     
        /// </summary>
        public string History { get; set; }
        
        /// <summary>
        /// 当前界面所从属的分类标签，它在 Html 表单中也称为 Tag。 根据 Category 数据表中的规定，每个
        /// Category 有一个数据库分配的自增的 Id 值，和一个特异的别名。而它还有显示名，不过显示名是否
        /// 特异不做考虑（当然，我们不鼓励显示名一样的分类，它会引起歧义）
        /// 
        /// 在 Edit 保存时的标准化过程中，它被规范成统一以 Alias 指代的形式。 假设（样表）
        /// 它是由逗号（英文）分割的 Alias 字符串 例如：'sys,policy'。
        /// </summary>
        public string Category { get; set; }

        public int Use { get; set; }
        public string Body { get; set; }

        [BindNever]
        [NotMapped]
        public List<Change> Changes { get; set; } = new List<Change>();
    }
}
