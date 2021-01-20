using System;

namespace WebApi.Entity
{
    public enum DownloadType
    {
        SourceFile = 1,
        ConvertedFile = 2,
        AnnotationFile = 3
    }


    public enum AnnotationStatus
    {
        /// <summary>
        /// 被删除的
        /// </summary>
        Deleted = -1,
        /// <summary>
        /// 修改批注后，旧的有效数据就成为此状态
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// 当前有效批注
        /// </summary>
        Available = 1

    }

    [Flags]
    public enum Role
    {
        noRole = 0,
        /// <summary>
        /// 
        /// </summary>
        superadmin = 1,
        /// <summary>
        ///撤稿执行人
        /// </summary>
        executer = 2,
        /// <summary>
        /// 审核人
        /// </summary>
        reviewer = 4,
        /// <summary>
        /// 申请撤稿人
        /// </summary>
        requester = 8
    }



    public enum CheckStatus
    {
        /// <summary>
        /// 执行撤稿失败
        /// </summary>
        ExecuteNG = -2,

        /// <summary>
        /// 审核失败
        /// </summary>
        ReviewNG = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitFor = 0,

        /// <summary>
        /// 审核处理成功
        /// </summary>
        ReviewOK = 1,

        /// <summary>
        /// 执行撤稿成功
        /// </summary>
        ExecuteOK = 2

    }
    /// <summary>
    /// 提交的每个项是否有错误
    /// </summary>
    public enum DetailStatus
    {

        /// <summary>
        /// 异常的错误的
        /// </summary>
        Abnormal = 0,

        /// <summary>
        /// 正常的
        /// </summary>
        Normal = 1,

    }

    /// <summary>
    /// 申请细节表中每行的类别
    /// </summary>
    public enum NodeKind
    {
        RequesterName = 0,
        RequesterPhoneNum = 1,
        RequesterOrganisation = 2,
        PersonIdentification = 3,
        ArticleName = 4,
        Author = 5,
        Publication = 6,
        PublishTime = 7,
        UnitIdentification = 8,
        CheckReport = 9,
        ArticleLocation = 10
    }

    /// <summary>
    /// 日志类别
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 登陆
        /// </summary>
        Login = 1,

        /// <summary>
        /// 注册
        /// </summary>
        Register = 2,
        /// <summary>
        /// <summary>
        /// 修改密码
        /// </summary>
        RePassword = 3,
        /// <summary>
        /// 提交
        /// </summary>
        SubmitRequest = 4,
        /// <summary>
        /// 审核
        /// </summary>
        RequestReview = 5,
        /// <summary>
        /// 执行撤稿
        /// </summary>
        RequestExecute = 6

    }
    /// <summary>
    /// 短信类别
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 注册
        /// </summary>
        Register = 1,

        /// <summary>
        /// 找回密码
        /// </summary>
        ReFindPassword = 2
        /// <summary>

    }

}
