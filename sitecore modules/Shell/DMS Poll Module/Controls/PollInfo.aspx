<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PollInfo.aspx.cs" Inherits="Sitecore.Modules.DMSPoll.Controls.PollInfo" %>
<%@ Import Namespace="Sitecore.Web.UI.WebControls"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <style type="text/css">
        .scDMSPollOptionResults
        {
            width: 280px;
            padding: 0.5em 0.5em 0.5em 0;
        }
        .scDMSPollOptionItemResult
        {
            font-size: 0.8em;
            padding: 0 0.3em 0.3em 0;
            text-align: left;
        }
        .scDMSPollOptionItemResultValue
        {
            color: #346183;
            font-weight: bold;
        }
        .scDMSPollVisualisation
        {
            padding-top: 1px;
            width: 100%;
            margin-right: 2em;
        }
        .scDMSPollPipe
        {
            float: left;
            background: url(../img/poll_pipe_part.png) repeat-x ;
            background-repeat: repeat-x;
            width: 90%;
        }
        .scDMSPollPipe IMG
        {
            margin-top: 1px;
            height: 13px;
        }
        .scDMSPollVisualisation IMG
        {
            margin-top: 1px;
            height: 13px;
        }
        .scDMSPollOptionResultsWrapper
        {
            border-top: solid 1px #999999;
            padding-left: 1em;
        }
        .scDMSPollHeader
        {
            margin: 2em 1em 1em 1em;
        }
        .scDMSPollIntro H1
        {
            font-size: 8pt;
            font-family: Tahoma;
        }
        .scDMSPollResultsWrapper
        {
            font-size: 8pt;
            font-family: Tahoma;
        }
        .scDMSPollWarning
        {
            background-color: #ffffe4;
            border-color: #9c9c9c #ffffff;
            font-family: Tahoma;
            font-size: 0.6em;
            font-weight: bold;
            padding: 0.5em 1em 0.5em 1em;
            width: 100%;
            border-style: solid none;
            border-width: 1px;
            margin-top: 0.3em;
        }
        .scDMSPollBar
        {
        	margin-top: 2em;
        }
    </style>
    <div class="scDMSPollBar" />
    <div runat="server" id="divCurrentPollClosed" visible="false" class="scDMSPollWarning">
        This poll is closed
    </div>
    <div runat="server" id="divArchivedPoll" visible="false" class="scDMSPollWarning">
        This poll is archived
    </div>
    <div class="scDMSPollResultsWrapper">
        <div class="scDMSPollHeader">
            <div class="scDMSPollIntro">
                <%=  FieldRenderer.Render(CurrentPoll.InnerItem, "Intro")%>
            </div>
        </div>
        <div class="scDMSPollOptionResultsWrapper">
            <div class="scDMSPollOptionResults">
                <% for (int i = 0; i < CurrentPollOptionItems.Length; i++) %>
                <% {%>
                <div class="scDMSPollOptionItemResult">
                    <span class="scDMSPollOptionItemName">
                        <%= FieldRenderer.Render(CurrentPollOptionItems[i].InnerItem, "Title")%>
                        <span class="scDMSPollOptionItemResultValue">
                            <%= CurrentPollOptionItems[i].GetVoteCountPercent().ToString("0")%>%</span>
                    </span>
                    <div class="scDMSPollVisualisation">
                        <div class="scDMSPollPipe">
                            <img src="/~/media/images/DMS Poll/PollLine.ashx?db=master" style="width: <%= CurrentPollOptionItems[i].GetVoteCountPercent().ToString("0")%>%"
                                alt="<%= CurrentPollOptionItems[i].GetVoteCountPercent().ToString("0")%>%" />
                        </div>
                        <div>
                            <img src="/~/media/images/DMS Poll/PollPipeRight.ashx?db=master" />
                        </div>
                    </div>
                </div>
                <% }%>
            </div>
            <div>
                Total:
                <%= CurrentPoll.VoteCount %>
                voices</div>
        </div>
    </div>
    </form>
</body>
</html>
