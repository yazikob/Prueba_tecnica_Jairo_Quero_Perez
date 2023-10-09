<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PruebaTecnicaJQP._Default" EnableViewState="true" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="center">
                    <asp:Label ID="lblFirstLoadTime" runat="server" CssClass="first-load-time"></asp:Label>
                    <section class="col" aria-labelledby="hostingTitle">
                        <p>
                            <asp:Table ID="tblClientes" runat="server" CssClass="table table-responsive"></asp:Table>
                            <asp:Button ID="btnCargaTabla" runat="server" Text="Carga Tabla" OnClick="btnCargaTabla_Click" />
                            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                        </p>
                    </section>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </main>

</asp:Content>
