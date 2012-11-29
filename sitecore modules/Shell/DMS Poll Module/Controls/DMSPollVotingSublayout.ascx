<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DMSPollVotingSublayout.ascx.cs"
    Inherits="Sitecore.Modules.DMSPoll.Controls.PollVotingSublayout" %>
<%@ Import Namespace="Sitecore.Modules.DMSPoll" %>
<%@ Import Namespace="Sitecore.Web.UI.WebControls" %><% if (this.IsPollExistsInCurrentLanguage && this.IsPollOptionsExists) %>
<% {%>

<script src="/sitecore modules/shell/DMS Poll Module/Poll.js" type="text/javascript"></script>

<%-- This div is template for voting form--%>
<div runat="server" id="divVoting" visible="false" class="scDMSPollVotingWrapper">
    <% for (int i = 0; i < PollOptionItems.Length; i++)
       { %>
    <div class="scDMSPollOptionItem">
        <input type="radio" id="<%= "fld_" + PollOptionItems[i].ID.ToString() %>" name="<%= ClientID %>" value="<%= PollOptionItems[i].ID.ToString() %>" />
        <label for="<%= PollOptionItems[i].ID.ToString() %>">
            <%= FieldRenderer.Render(PollOptionItems[i].InnerItem, "Title")%>
        </label>
    </div>
    <% }%>
</div>
<%-- This div is template for results form--%>
<div runat="server" id="divResults" visible="false" class="scDMSPollResultsWrapper">
    <div class="scDMSPollOptionResults">
        <% for (int i = 0; i < PollOptionItems.Length; i++)
           { %>
        <div class="scDMSPollOptionItemResult">
            <span class="scDMSPollOptionItemName">
                <%= FieldRenderer.Render(PollOptionItems[i].InnerItem, "Title")%>
                <span class="scDMSPollOptionItemResultValue">
                    <%= PollOptionItems[i].GetVoteCountPercent().ToString("0")%>%</span> </span>
            <div class="scDMSPollVisualisation">
                <div class="scDMSPollPipe">
                    <img src="/~/media/images/DMS Poll/PollLine%20.ashx" style='width: <%= PollOptionItems[i].GetVoteCountPercent().ToString("0") %>%' alt='<%= PollOptionItems[i].GetVoteCountPercent().ToString("0") %>%' />
                </div>
                <div>
                    <img src="/~/media/images/DMS Poll/PollPipeRight%20.ashx" alt="PollPipeRight" />
                </div>
            </div>
        </div>
        <% }%>
    </div>
    <div runat="server" id="divThankYou" class="scDMSPollThankYou">
        <%= FieldRenderer.Render(CurrentPoll.InnerItem, "Thank you")%>
    </div>
</div>
<div id="module_poll" class="module">
    <div class="module_inner_1">
        <div class="module_inner_2">
            <div class="scDMSPollOptionIntro">
                <%= FieldRenderer.Render(CurrentPoll.InnerItem, "Intro")%></div>
            <%-- Do not remove this div. It is serves as placeholder for getting by AJAX content --%>
            <div runat="server" id="divContent" class="scDMSPollContent">
            </div>
            <div runat="server" id="divButtons" class="scDMSButtons">
                <a id="<%= ClientID%>_PlaceVote" href="javascript:DMSPoll.RaiseCallback()">
                    <%= FieldRenderer.Render(CurrentPoll.InnerItem, "Submit vote")%>
                </a>
            </div>
            <div id="divCookiesRequired" class="scDMSCookiesRequired">
                <%= FieldRenderer.Render(CurrentPoll.InnerItem, PollConstants.PollItemCookiesRequiredText)%>
            </div>
        </div>
    </div>
</div>
<% }%>