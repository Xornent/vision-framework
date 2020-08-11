using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Vision.Difference.Builder;
using Vision.Models;

namespace Vision.Difference {

    public class DifferParser {
        public DifferParser() {

        }

        public string Build(string diff, int version = int.MaxValue) {
            string gen = "";
            List<string> content = new List<string>();
            string[] lines = diff.Split('\n');
            for(int i=0;i<lines.Count();i++) {
                string current = lines[i];

                // 标记版本列由 : 开头
                
                if(current.StartsWith(":")) {
                    int ver = 0;
                    bool success = int.TryParse(current.Remove(0, 1).Trim(), out ver);
                    if (success) {
                        if (ver > version) break;
                    } else return gen;
                }

                if(current.StartsWith("@")) {

                    // 由 @ 引导的信息列被忽略

                } else {
                    string cmd = "";
                    string text = "";
                    bool hasbracket = false;
                    bool bracketclosed = false;
                    foreach (char c in current) {
                        if (c == '(' && bracketclosed == false) {
                            hasbracket = true;
                        } else if (c == ')' && hasbracket) {
                            bracketclosed = true;
                            hasbracket = false;
                        } else {
                            if (hasbracket) {
                                if (!bracketclosed) {

                                    // 这时我们在括号内部，解析命令：

                                    cmd = cmd + c;
                                }
                            } else {
                                text = text + c;

                            }
                        }
                    }
                    string[] cmdList = cmd.ToLower().Trim().Split(' ');
                    if(cmdList[0] == "-" ||
                        cmdList[0] == "del") {
                        int count = Convert.ToInt32(cmdList[1]);
                        content.RemoveAt(count - 1);
                    } else if(cmdList[0] == "+" ||
                        cmdList[0] == "ins") {
                        int count = Convert.ToInt32(cmdList[1]);
                        content.Insert(count - 1, text);
                    }
                }
            }

            foreach(var line in content) {
                gen = gen + line + "\n";
            }
            return gen.Trim('\n');
        }

        public int GetVersion(string diff) {
            string[] lines = diff.Split('\n');
            int version = 0;
            for (int i = 0; i < lines.Count(); i++) {
                string current = lines[i];

                // 标记版本列由 : 开头

                if (current.StartsWith(":")) {
                    int ver = 0;
                    bool success = int.TryParse(current.Remove(0, 1).Trim(), out ver);
                    if (success) {
                        if (ver > version) version = ver;
                    }
                }
            }
            return version;
        }

        public List<Change> GetChanges(string diff) {
            string[] lines = diff.Split('\n');
            int version = 0;
            bool logged = true;
            Change c = new Change();
            List<Change> result = new List<Change>();
            bool startread = false;
            for (int i = 0; i < lines.Count(); i++) {
                string current = lines[i];

                // 标记版本列由 : 开头
                if (current.StartsWith(":")) {
                    if (!logged) {
                        c.Version = version;
                        result.Add(c);
                        c = new Change();
                    }
                    logged = true;
                    int ver = 0;
                    bool success = int.TryParse(current.Remove(0, 1).Trim(), out ver);
                    if (success) {
                        version = ver;
                        startread = true;
                        continue;
                    }
                }

                if(startread) {
                    if(current.StartsWith("@")) {
                        logged = false;
                        string[] secs = current.Split(' ');
                        string tag = secs[0].Replace("@", "").Trim();
                        switch (tag) {
                            case "user":
                                c.User = secs[1].Trim();
                                break;
                            case "datetime":
                                c.Post = DateTime.Parse(current.Replace("@datetime","").Trim());
                                break;
                            case "summary":
                                c.Summary = secs[1].Trim();
                                break;
                            case "delete":
                                c.Type = (int)ChangeTag.Delete;
                                break;
                            case "publish":
                                c.Type = (int)ChangeTag.Republish;
                                break;
                            default:
                                break;
                        }
                    } else {
                        startread = false;
                    }
                }
            }
            result.Add(c);
            return result;
        }

        public string Generate(string src, string dest) {
            string gen = "";
            var diff = InlineBuilder.Diff(src, dest);
            int line = 1;
            foreach(var l in diff.Lines) {
                switch (l.Type) {
                    case ChangeType.Deleted:
                        gen = gen + "(- " + line + ")\n";
                        line--;
                        break;
                    case ChangeType.Inserted:
                        gen = gen + "(+ " + line + ")" + l.Text +"\n";
                        break;
                    default:
                        break;
                }
                line++;
            }
            return gen.Trim('\n');
        }
    }
}
