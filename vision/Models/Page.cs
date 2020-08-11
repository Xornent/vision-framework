using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Vision.Models {

    /// <summary>
    /// Page 表
    /// </summary>
    public class Page {

        /// <summary>
        /// 这是在创建页面（第一次存入数据库时）由 Title 自动生成的 MD5 16 进制字符串。
        /// 如果 MD5 运行错误，返回常量 '00000000000000000000000000000000'。 
        /// 
        /// Hash 列在数据库中也是索引列，在已知搜索的标题时，调用 <see cref="Cryptography.MD5.Encrypt(string)"/> 
        /// 进行摘要转换，可以从此列中粗略查找目标对象。
        /// </summary>
        [MaxLength(32)]
        public string Hash { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 页面标题的标题限定字符串。
        /// </summary>
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string Namespace { get; set; }

        /// <summary>
        /// 参见 <see cref="ProtectiveLevel"/>
        /// </summary>
        public int Level { get; set; }

        [BindNever]
        [NotMapped]
        public List<string> Models { get; set; } = new List<string>();
    }

    /// <summary>
    /// 可编辑的保护程度
    /// 
    /// Vision 的条目的读权限是无条件的，任何人（包括合理使用的机器人）都可以读取条目；但是为了
    /// 保证条目的质量和防止恶意编辑，我们对写权限做出控制。 写权限的程度由此值表示。此值越大，
    /// 表示编辑需要的条件和限制越多。
    /// 
    /// 用户的 6 类定义
    /// 
    ///   * 认证的机器人：对于许多数据库式的条目，认证的机器人编辑可以节省很多人力物力，也可以避免
    ///     粗心导致的错误（比如城市的地理位置，化学品的理化常数等）
    ///     
    ///   * 非用户的 IP 地址：常规情况下，每个人都可以编辑条目，无论你是否是 Vision 的用户。一个
    ///     没有注册成用户的编辑会显示默认头像和编辑机器的 IP 地址作为名称。
    ///     
    ///   * 用户：注册成为 Vision 账户的人，它们提供了更详细的个人资料。可参加涉及编者责任的编辑。
    ///   
    ///   * 认证的用户：在某一或某几个领域受到信任，可以编辑良好条目的人。
    ///   
    ///   * 管理员：管理员有用户提交申请，批准通过而来；它有更大的编辑自由度，也有更大的责任。 包括
    ///     检查普通用户的词条是否符合规范等事务。
    ///     
    ///   * 开发者：Vision， Vision 的子项目和所有分支，派生项目的编写和维护人员。
    /// </summary>
    public enum ProtectiveLevel : int {

        /// <summary>
        /// 任何人，包括认证的机器人，非用户的 IP 地址，用户，认证的用户，管理员和开发者都可以编辑。
        /// 绝大多数界面应该归于此类，除非恶意的机器人操作不可容忍。
        /// </summary>
        Free,

        /// <summary>
        /// 除了机器人外的所有用户都可编辑（详见用户的 6 类定义，在 ProtectiveLevel 的文档注释）
        /// （非用户的 IP 地址，用户，认证的用户，管理员和开发者可以编辑）
        /// 恶意的机器人操作不可容忍时，绝大多数界面应该归于此类。
        /// </summary>
        Person,

        /// <summary>
        /// 只有注册成为用户的人才能编辑 （用户，认证的用户，管理员和开发者可以编辑）
        /// 当页面涉及争议话题和特殊话题，需要责任承担人时，用户的信息需要被有关部门知晓。这时才能
        /// 使用本权限设置。
        /// </summary>
        User,

        /// <summary>
        /// 条目的专业性较强，且已经到达无词法语法错误，无标点符号错误的语法稳定状态，只需要专业人员
        /// 对条目进行维护。 （认证的用户，管理员和开发者可以编辑）
        /// 
        /// 注意，请谨慎使用这个权限。因为绝大部分条目还没有到达语法稳定状态。
        /// </summary>
        Authorized,

        /// <summary>
        /// 条目是 Vision 政策和指导等特殊界面 （管理员和开发者可以编辑）
        /// </summary>
        Administrator,

        /// <summary>
        /// 特殊界面 （只有开发者可以编辑）
        /// </summary>
        Developer
    }
}
