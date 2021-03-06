﻿'==============================================================================
'
' EveHQ - An Eve-Online™ character assistance application
' Copyright © 2005-2015  EveHQ Development Team
'
' This file is part of EveHQ.
'
' The source code for EveHQ is free and you may redistribute 
' it and/or modify it under the terms of the MIT License. 
'
' Refer to the NOTICES file in the root folder of EVEHQ source
' project for details of 3rd party components that are covered
' under their own, separate licenses.
'
' EveHQ is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the MIT 
' license below for details.
'
' ------------------------------------------------------------------------------
'
' The MIT License (MIT)
'
' Copyright © 2005-2015  EveHQ Development Team
'
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in
' all copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
' THE SOFTWARE.
'
' ==============================================================================

Imports System.Net
Imports System.Text
Imports EveHQ.Core
Imports System.Web
Imports System.Text.RegularExpressions

Namespace Classes

    Public Class IGBData

        Shared currentFitting As Fitting
        Shared hqfPilotName As String

        Shared Function Response(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder
            strHTML.Append(IGB.IGBHTMLHeader(context, "EveHQFitter", 0))
            strHTML.Append(HQFMenu(context))
            Select Case context.Request.Url.AbsolutePath.ToUpper
                Case "/EVEHQFITTER", "/EVEHQFITTER/"
                    strHTML.Append(MainPage(context))
                Case "/EVEHQFITTER/BROWSEFITTINGS", "/EVEHQFITTER/BROWSEFITTINGS/"
                    strHTML.Append(BrowseFittings(context))
                Case "/EVEHQFITTER/EVEFITTING", "/EVEHQFITTER/EVEFITTING/"
                    strHTML.Append(EveFitting(context))
                Case "/EVEHQFITTER/SAVEEVEFITTING", "/EVEHQFITTER/SAVEEVEFITTING/"
                    strHTML.Append(SaveEveFitting(context))
                Case "/EVEHQFITTER/PAGE2", "/EVEHQFITTER/PAGE2/"
                    strHTML.Append(HQFPage2(context))
            End Select
            strHTML.Append(IGB.IGBHTMLFooter(context))
            Return strHTML.ToString
        End Function

        Shared Function HQFMenu(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder
            strHTML.Append("<a href=/EVEHQFitter>HQF Home</a>  |  <a href=/EVEHQFitter/BrowseFittings>Browse HQF Fittings</a>  |  <a href=/EVEHQFitter/Page2>HQF Page 2</a><br /><hr>")
            Return strHTML.ToString
        End Function

        Shared Function MainPage(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder
            ' Check if we are trusted and get the Eve system name
            If context.Request.Headers("EVE_TRUSTED") = Nothing Then
                strHTML.Append("<p>This section of the IGB requires access via the Eve IGB.</p>")
            Else
                If context.Request.Headers("EVE_TRUSTED") = "no" Then
                    strHTML.Append("<p>This website is not trusted so cannot interact with </p>")
                Else
                    hqfPilotName = context.Request.Headers("EVE_CHARNAME")
                    If FittingPilots.HQFPilots.ContainsKey(hqfPilotName) = False Then
                        If FittingPilots.HQFPilots.Count = 0 Then
                            hqfPilotName = ""
                        Else
                            hqfPilotName = FittingPilots.HQFPilots.Keys(0).ToString
                        End If
                    End If
                End If
                If hqfPilotName = "" Then
                    strHTML.Append("<p>Unable to find a suitable pilot to display skills. Check HQF is enabled and has been loaded.</p>")
                Else
                    strHTML.Append("<p>Fitting statistics will be for " & hqfPilotName & " unless otherwise stated</p>")
                    strHTML.Append("</p>")
                    strHTML.Append("<form method=""GET"" action=""/EveHQFitter/EveFitting"">")
                    strHTML.Append("Paste Eve Fitting:  ")
                    strHTML.Append("<input type=""text"" name=""fitting"">")
                    strHTML.Append("<input type=""submit"" value=""Submit""></form><hr><br>")
                End If
            End If

            Return strHTML.ToString
        End Function

        Shared Function BrowseFittings(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder

            Dim shipName As String
            Dim fittingName As String

            strHTML.Append("<table width=100% border=1>")
            Const LastShip As String = ""
            For Each fitting As String In Fittings.FittingList.Keys
                shipName = Fittings.FittingList(fitting).ShipName
                fittingName = Fittings.FittingList(fitting).FittingName

                If shipName <> LastShip Then
                    strHTML.Append("<tr>")
                    strHTML.Append("<td width=20px>&nbsp;</td>")
                    strHTML.Append("<td width=32px><img src='http://image.eveonline.com/Render/" & ShipLists.ShipListKeyName(shipName) & "_32.png' width=32px height=32px></td>")
                    strHTML.Append("<td colspan=5><font size='3'>" & shipName & "</font></td>")
                    strHTML.Append("</tr>")
                End If

                strHTML.Append("<tr>")
                strHTML.Append("<td width=20px>&nbsp;</td>")
                strHTML.Append("<td width=32px>&nbsp;</td>")
                strHTML.Append("<td width=20px>&nbsp;</td>")
                strHTML.Append("<td width=300px>" & fittingName & "</td>")
                strHTML.Append("<td width=300px>" & Fittings.FittingList(fitting).PilotName & "</td>")
                strHTML.Append("<td><a href='EveFitting?fitting=" & DNAFitting.GetDNAFromFitting(Fittings.FittingList(fitting), True) & "'>Display Stats</a></td>")
                strHTML.Append("<td><button type=""button"" onclick=""CCPEVE.showFitting('" & DNAFitting.GetDNAFromFitting(Fittings.FittingList(fitting), False) & "')"">Show Fitting</button></td>")
                strHTML.Append("</tr>")
            Next
            strHTML.Append("</table>")

            Return strHTML.ToString
        End Function

        Shared Function EveFitting(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder

            If context.Request.QueryString.Count = 0 Or context.Request.QueryString.Item("fitting") = "" Then
                strHTML.Append("<p>Eve fitting has no content - please check data!</p>")
            Else
                Try

                    ' Try and establish some form parameters for the fitting
                    Dim fittingURL As String = HttpUtility.UrlDecode(context.Request.QueryString.Item("fitting"))
                    If HttpUtility.UrlDecode(context.Request.QueryString.Item("fitting2")) <> "" Then
                        fittingURL = HttpUtility.UrlDecode(context.Request.QueryString.Item("fitting2"))
                    End If

                    '   "\S+=["']?((?:.(?!["']?\s+(?:\S+)=|[>"']))+.)["']?>(.*?)\<\/url\>"
                    Dim newRegex As New Regex("\S+=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?>(.*?)\<\/url\>")
                    Dim newMatch As Match = newRegex.Match(fittingURL)
                    If newMatch.Success = False Then
                        strHTML.Append("<p>Unable to parse fitting URL</p>")
                    Else
                        Dim fittingDNA As String = newMatch.Groups.Item(1).Value
                        Dim fittingName As String = newMatch.Groups.Item(2).Value
                        'strHTML.Append("<p>Fitting DNA: " & fittingDNA & "</p>")
                        'strHTML.Append("<p>Fitting Name: " & fittingName & "</p>")
                        'For Each qs As String In context.Request.QueryString
                        '    strHTML.Append("<p>" & qs & ": " & Web.HttpUtility.UrlDecode(context.Request.QueryString.Item(qs)) & "</p>")
                        'Next

                        ' Set the fitting details and apply it
                        currentFitting = DNAFitting.GetFittingFromEveDNA(fittingDNA, fittingName)

                        If HttpUtility.UrlDecode(context.Request.QueryString.Item("pilot")) <> "" Then
                            currentFitting.PilotName = HttpUtility.UrlDecode(context.Request.QueryString.Item("pilot"))
                        Else
                            currentFitting.PilotName = hqfPilotName
                        End If
                        currentFitting.UpdateBaseShipFromFitting()

                        If HttpUtility.UrlDecode(context.Request.QueryString.Item("implants")) <> "" Then
                            currentFitting.ImplantGroup = HttpUtility.UrlDecode(context.Request.QueryString.Item("implants"))
                        Else
                            currentFitting.ImplantGroup = FittingPilots.HQFPilots(currentFitting.PilotName).ImplantName(0)
                        End If
                        FittingPilots.HQFPilots(currentFitting.PilotName).ImplantName(0) = currentFitting.ImplantGroup

                        If HttpUtility.UrlDecode(context.Request.QueryString.Item("damage")) <> "" Then
                            currentFitting.BaseShip.DamageProfile = HQFDamageProfiles.ProfileList(HttpUtility.UrlDecode(context.Request.QueryString.Item("damage")))
                        Else
                            currentFitting.BaseShip.DamageProfile = HQFDamageProfiles.ProfileList("<Omni-Damage>")
                        End If

                        ' Check for charges
                        For slot As Integer = 1 To currentFitting.BaseShip.HiSlots
                            If currentFitting.BaseShip.HiSlot(slot) IsNot Nothing Then
                                Dim chargeName As String = HttpUtility.UrlDecode(context.Request.QueryString.Item(currentFitting.BaseShip.HiSlot(slot).Name))
                                If chargeName <> "" Then
                                    ' Load the charge
                                    Dim charge As ShipModule = ModuleLists.ModuleList(ModuleLists.ModuleListName(chargeName)).Clone
                                    currentFitting.BaseShip.HiSlot(slot).LoadedCharge = charge
                                Else
                                    If currentFitting.BaseShip.HiSlot(slot).Charges.Count <> 0 Then
                                        Dim firstCharge As ShipModule = ModuleLists.ModuleList(ModuleLists.ModuleListName(currentFitting.BaseShip.HiSlot(slot).GetChargeList.Keys(0))).Clone
                                        currentFitting.BaseShip.HiSlot(slot).LoadedCharge = firstCharge
                                    End If
                                End If
                            End If
                        Next
                        For slot As Integer = 1 To currentFitting.BaseShip.MidSlots
                            If currentFitting.BaseShip.MidSlot(slot) IsNot Nothing Then
                                Dim chargeName As String = HttpUtility.UrlDecode(context.Request.QueryString.Item(currentFitting.BaseShip.MidSlot(slot).Name))
                                If chargeName <> "" Then
                                    ' Load the charge
                                    Dim charge As ShipModule = ModuleLists.ModuleList(ModuleLists.ModuleListName(chargeName)).Clone
                                    currentFitting.BaseShip.MidSlot(slot).LoadedCharge = charge
                                Else
                                    If currentFitting.BaseShip.MidSlot(slot).Charges.Count <> 0 Then
                                        Dim firstCharge As ShipModule = ModuleLists.ModuleList(ModuleLists.ModuleListName(currentFitting.BaseShip.MidSlot(slot).GetChargeList.Keys(0))).Clone
                                        currentFitting.BaseShip.MidSlot(slot).LoadedCharge = firstCharge
                                    End If
                                End If
                            End If
                        Next

                        currentFitting.ApplyFitting()

                        strHTML.Append("<form method=""GET"" action=""/EveHQFitter/EveFitting"">")

                        ' Draw the various drop-downs
                        strHTML.Append("<table width=100% border=0><tr>")
                        strHTML.Append("<td width=30%>Pilot:")
                        strHTML.Append("<select name=pilot style='width: 100%; height:16px; font:normal 10px Arial, Tahoma; text-decoration:none;'>")
                        For Each pilot As String In FittingPilots.HQFPilots.Keys
                            strHTML.Append("<option ")
                            If pilot = currentFitting.PilotName Then
                                strHTML.Append("selected='selected'")
                            End If
                            strHTML.Append(">" & pilot & "</option>")
                        Next
                        strHTML.Append("</select></td>")
                        strHTML.Append("<td width=30%>Implants:")
                        strHTML.Append("<select name=implants style='width: 100%; height:16px; font:normal 10px Arial, Tahoma; text-decoration:none;'>")
                        For Each implant As String In PluginSettings.HQFSettings.ImplantGroups.Keys
                            strHTML.Append("<option ")
                            If implant = currentFitting.ImplantGroup Then
                                strHTML.Append("selected='selected'")
                            End If
                            strHTML.Append(">" & implant & "</option>")
                        Next
                        strHTML.Append("</select></td>")
                        strHTML.Append("<td width=30%>Profile:")
                        strHTML.Append("<select name=damage style='width: 100%; height:16px; font:normal 10px Arial, Tahoma; text-decoration:none;'>")
                        For Each profile As String In HQFDamageProfiles.ProfileList.Keys
                            strHTML.Append("<option ")
                            If profile = currentFitting.BaseShip.DamageProfile.Name Then
                                strHTML.Append("selected='selected'")
                            End If
                            strHTML.Append(">" & HttpUtility.HtmlEncode(profile) & "</option>")
                        Next
                        strHTML.Append("</select></td>")
                        strHTML.Append("</tr></table>")

                        ' Collect similar modules
                        Dim hiSlots As New SortedList(Of String, Integer)
                        Dim midSlots As New SortedList(Of String, Integer)
                        Dim lowSlots As New SortedList(Of String, Integer)
                        Dim rigSlots As New SortedList(Of String, Integer)
                        Dim drones As New SortedList(Of String, Integer)
                        For slot As Integer = 1 To currentFitting.FittedShip.HiSlots
                            If currentFitting.FittedShip.HiSlot(slot) IsNot Nothing Then
                                If hiSlots.ContainsKey(currentFitting.FittedShip.HiSlot(slot).Name) = False Then
                                    hiSlots.Add(currentFitting.FittedShip.HiSlot(slot).Name, 1)
                                Else
                                    hiSlots(currentFitting.FittedShip.HiSlot(slot).Name) += 1
                                End If
                            End If
                        Next
                        For slot As Integer = 1 To currentFitting.FittedShip.MidSlots
                            If currentFitting.FittedShip.MidSlot(slot) IsNot Nothing Then
                                If midSlots.ContainsKey(currentFitting.FittedShip.MidSlot(slot).Name) = False Then
                                    midSlots.Add(currentFitting.FittedShip.MidSlot(slot).Name, 1)
                                Else
                                    midSlots(currentFitting.FittedShip.MidSlot(slot).Name) += 1
                                End If
                            End If
                        Next
                        For slot As Integer = 1 To currentFitting.FittedShip.LowSlots
                            If currentFitting.FittedShip.LowSlot(slot) IsNot Nothing Then
                                If lowSlots.ContainsKey(currentFitting.FittedShip.LowSlot(slot).Name) = False Then
                                    lowSlots.Add(currentFitting.FittedShip.LowSlot(slot).Name, 1)
                                Else
                                    lowSlots(currentFitting.FittedShip.LowSlot(slot).Name) += 1
                                End If
                            End If
                        Next
                        For slot As Integer = 1 To currentFitting.FittedShip.RigSlots
                            If currentFitting.FittedShip.RigSlot(slot) IsNot Nothing Then
                                If rigSlots.ContainsKey(currentFitting.FittedShip.RigSlot(slot).Name) = False Then
                                    rigSlots.Add(currentFitting.FittedShip.RigSlot(slot).Name, 1)
                                Else
                                    rigSlots(currentFitting.FittedShip.RigSlot(slot).Name) += 1
                                End If
                            End If
                        Next
                        For Each dbi As DroneBayItem In currentFitting.FittedShip.DroneBayItems.Values
                            If drones.ContainsKey(dbi.DroneType.Name) = False Then
                                drones.Add(dbi.DroneType.Name, dbi.Quantity)
                            Else
                                drones(dbi.DroneType.Name) += dbi.Quantity
                            End If
                        Next

                        ' Draw outline table
                        strHTML.Append("<table width=100% border=0>")
                        strHTML.Append("<tr><td width=50% valign=top>")

                        ' Draw ship
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr><td width=64px><img src='http://image.eveonline.com/Render/" & ShipLists.ShipListKeyName(currentFitting.ShipName) & "_64.png' width=64px height=64px>")
                        strHTML.Append("</td><td valign=top style='font:normal 16px  Arial, Tahoma;'>")
                        strHTML.Append("<div style='font:normal 18px  Arial, Tahoma;'><b>" & currentFitting.ShipName & "</b></div><br />")
                        strHTML.Append("<div style='font:normal 12px  Arial, Tahoma;'>[" & fittingName & "]</div></td></tr>")
                        strHTML.Append("</table>")

                        ' Draw fitting table
                        strHTML.Append("<table width=100% border=1>")

                        If currentFitting.FittedShip.HiSlots > 0 Then
                            strHTML.Append("<tr><td colspan=2 bgcolor=#111111><b>High Slots</b></td></tr>")
                            Dim modCount As Integer = 0
                            For Each slot As String In hiSlots.Keys
                                Dim chargeList As SortedList(Of String, String) = ModuleLists.ModuleList(ModuleLists.ModuleListName(slot)).GetChargeList
                                If chargeList.Count > 0 Then
                                    If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(slot))) Then
                                        strHTML.Append("<tr><td width=50% bgcolor=#333333>" & hiSlots(slot) & "x " & slot & "</td>")
                                    Else
                                        strHTML.Append("<tr><td width=50% bgcolor=#333333><font color=""red"">" & hiSlots(slot) & "x " & slot & "</font></td>")
                                    End If
                                    strHTML.Append("<td width=50% bgcolor=#333333>")
                                    strHTML.Append("<select name='" & slot & "' style='width: 100%; height:14px; font:normal 10px Arial, Tahoma; text-decoration:none;'>")
                                    For Each charge As String In chargeList.Keys
                                        strHTML.Append("<option ")
                                        If charge = HttpUtility.UrlDecode(context.Request.QueryString.Item(slot)) Then
                                            strHTML.Append("selected='selected'")
                                        End If
                                        strHTML.Append(">" & charge & "</option>")
                                    Next
                                    strHTML.Append("</select></td></tr>")
                                Else
                                    If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(slot))) Then
                                        strHTML.Append("<tr><td colspan=2 bgcolor=#333333>" & hiSlots(slot) & "x " & slot & "</td></tr>")
                                    Else
                                        strHTML.Append("<tr><td colspan=2 bgcolor=#333333><font color=""red"">" & hiSlots(slot) & "x " & slot & "</font></td></tr>")
                                    End If
                                End If
                                modCount += hiSlots(slot)
                            Next
                            If modCount < currentFitting.FittedShip.HiSlots Then
                                ' ReSharper disable once RedundantAssignment - Incorrect warning by R#
                                For emptySlot As Integer = 1 To currentFitting.FittedShip.HiSlots - modCount
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>[Empty]</td></tr>")
                                Next
                            End If
                        End If

                        If currentFitting.FittedShip.MidSlots > 0 Then
                            strHTML.Append("<tr><td colspan=2 bgcolor=#111111><b>Mid Slots</b></td></tr>")
                            Dim modCount As Integer = 0
                            For Each slot As String In midSlots.Keys
                                Dim chargeList As SortedList(Of String, String) = ModuleLists.ModuleList(ModuleLists.ModuleListName(slot)).GetChargeList
                                If chargeList.Count > 0 Then
                                    If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(slot))) Then
                                        strHTML.Append("<tr><td width=50% bgcolor=#333333>" & midSlots(slot) & "x " & slot & "</td>")
                                    Else
                                        strHTML.Append("<tr><td width=50% bgcolor=#333333><font color=""red"">" & midSlots(slot) & "x " & slot & "</font></td>")
                                    End If
                                    strHTML.Append("<td width=50% bgcolor=#333333>")
                                    strHTML.Append("<select name='" & slot & "' style='width: 100%; height:14px; font:normal 10px Arial, Tahoma; text-decoration:none;'>")
                                    For Each charge As String In chargeList.Keys
                                        strHTML.Append("<option ")
                                        If charge = HttpUtility.UrlDecode(context.Request.QueryString.Item(slot)) Then
                                            strHTML.Append("selected='selected'")
                                        End If
                                        strHTML.Append(">" & charge & "</option>")
                                    Next
                                    strHTML.Append("</select></td></tr>")
                                Else
                                    If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(slot))) Then
                                        strHTML.Append("<tr><td colspan=2 bgcolor=#333333>" & midSlots(slot) & "x " & slot & "</td></tr>")
                                    Else
                                        strHTML.Append("<tr><td colspan=2 bgcolor=#333333><font color=""red"">" & midSlots(slot) & "x " & slot & "</font></td></tr>")
                                    End If
                                End If
                                modCount += midSlots(slot)
                            Next
                            If modCount < currentFitting.FittedShip.MidSlots Then
                                ' ReSharper disable once RedundantAssignment - Incorrect warning by R#
                                For emptySlot As Integer = 1 To currentFitting.FittedShip.MidSlots - modCount
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>[Empty]</td></tr>")
                                Next
                            End If
                        End If

                        If currentFitting.FittedShip.LowSlots > 0 Then
                            strHTML.Append("<tr><td colspan=2 bgcolor=#111111><b>Low Slots</b></td></tr>")
                            Dim modCount As Integer = 0
                            For Each slot As String In lowSlots.Keys
                                If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(slot))) Then
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>" & lowSlots(slot) & "x " & slot & "</td></tr>")
                                Else
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333><font color=""red"">" & lowSlots(slot) & "x " & slot & "</font></td></tr>")
                                End If
                                modCount += lowSlots(slot)
                            Next
                            If modCount < currentFitting.FittedShip.LowSlots Then
                                ' ReSharper disable once RedundantAssignment - Incorrect warning by R#
                                For emptySlot As Integer = 1 To currentFitting.FittedShip.LowSlots - modCount
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>[Empty]</td></tr>")
                                Next
                            End If
                        End If

                        If currentFitting.FittedShip.RigSlots > 0 Then
                            strHTML.Append("<tr><td colspan=2 bgcolor=#111111><b>Rig Slots</b></td></tr>")
                            Dim modCount As Integer = 0
                            For Each slot As String In rigSlots.Keys
                                If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(slot))) Then
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>" & rigSlots(slot) & "x " & slot & "</td></tr>")
                                Else
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333><font color=""red"">" & rigSlots(slot) & "x " & slot & "</font></td></tr>")
                                End If
                                modCount += rigSlots(slot)
                            Next
                            If modCount < currentFitting.FittedShip.RigSlots Then
                                ' ReSharper disable once RedundantAssignment - Incorrect warning by R#
                                For emptySlot As Integer = 1 To currentFitting.FittedShip.RigSlots - modCount
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>[Empty]</td></tr>")
                                Next
                            End If
                        End If

                        If currentFitting.FittedShip.SubSlots > 0 Then
                            strHTML.Append("<tr><td colspan=2 bgcolor=#111111><b>Subsystem Slots</b></td></tr>")
                            For slot As Integer = 1 To currentFitting.FittedShip.SubSlots
                                If currentFitting.FittedShip.SubSlot(slot) IsNot Nothing Then
                                    If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), currentFitting.FittedShip.SubSlot(slot)) Then
                                        strHTML.Append("<tr><td colspan=2 bgcolor=#333333>" & currentFitting.FittedShip.SubSlot(slot).Name & "</td></tr>")
                                    Else
                                        strHTML.Append("<tr><td colspan=2 bgcolor=#333333><font color=""red"">" & currentFitting.FittedShip.SubSlot(slot).Name & "</font></td></tr>")
                                    End If
                                Else
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>[Empty]</td></tr>")
                                End If
                            Next
                        End If

                        If currentFitting.FittedShip.DroneBayItems.Count > 0 Then
                            strHTML.Append("<tr><td colspan=2 bgcolor=#111111><b>Drone Bay</b></td></tr>")
                            For Each droneName As String In drones.Keys
                                If Engine.IsUsable(FittingPilots.HQFPilots(currentFitting.PilotName), ModuleLists.ModuleList(ModuleLists.ModuleListName(droneName))) Then
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333>" & drones(droneName).ToString("N0") & "x " & droneName & "</td></tr>")
                                Else
                                    strHTML.Append("<tr><td colspan=2 bgcolor=#333333><font color=""red"">" & drones(droneName).ToString("N0") & "x " & droneName & "</font></td></tr>")
                                End If
                            Next
                        End If

                        strHTML.Append("</table>")
                        strHTML.Append("</td>")

                        ' Write stats
                        strHTML.Append("<td width=50% valign=top>")

                        ' Write fitting stats
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr bgcolor=#111111><td colspan=5><b>Fitting</b></td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>CPU</b></td><td colspan=4>" & currentFitting.FittedShip.CPUUsed.ToString("N2") & " / " & currentFitting.FittedShip.CPU.ToString("N2") & "</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>PG</b></td><td colspan=4>" & currentFitting.FittedShip.PGUsed.ToString("N2") & " / " & currentFitting.FittedShip.PG.ToString("N2") & "</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Calibration</b></td><td colspan=4>" & currentFitting.FittedShip.CalibrationUsed.ToString("N2") & " / " & currentFitting.FittedShip.Calibration.ToString("N2") & "</td></tr>")
                        strHTML.Append("</table>")

                        ' Write defence stats
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr bgcolor=#111111><td colspan=5><b>Defence</b></td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Shield HP</b></td><td colspan=2>" & currentFitting.FittedShip.ShieldCapacity.ToString("N0") & " (Effective: " & currentFitting.FittedShip.EffectiveShieldHP.ToString("N0") & ")</td><td width=20%><b>Recharge</b></td><td>" & currentFitting.FittedShip.ShieldRecharge.ToString("N2") & "s</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Armor HP</b></td><td colspan=4>" & currentFitting.FittedShip.ArmorCapacity.ToString("N0") & " (Effective: " & currentFitting.FittedShip.EffectiveArmorHP.ToString("N0") & ")</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Hull HP</b></td><td colspan=4>" & currentFitting.FittedShip.StructureCapacity.ToString("N0") & " (Effective: " & currentFitting.FittedShip.EffectiveStructureHP.ToString("N0") & ")</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Total EHP</b></td><td colspan=4>" & currentFitting.FittedShip.EffectiveHP.ToString("N0") & " (Eve: " & currentFitting.FittedShip.EveEffectiveHP.ToString("N0") & ")</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Tank Ability</b></td><td colspan=4>" & CDbl(currentFitting.FittedShip.Attributes(10062)).ToString("N2") & " DPS</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Shield Resists</b></td><td width=20%>EM: " & currentFitting.FittedShip.ShieldEMResist.ToString("N2") & "%</td> <td width=20%>Exp: " & currentFitting.FittedShip.ShieldExResist.ToString("N2") & "%</td><td width=20%>Kin: " & currentFitting.FittedShip.ShieldKiResist.ToString("N2") & "%</td><td width=20%>Th: " & currentFitting.FittedShip.ShieldThResist.ToString("N2") & "%</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Armor Resists</b></td><td width=20%>EM: " & currentFitting.FittedShip.ArmorEMResist.ToString("N2") & "%</td> <td width=20%>Exp: " & currentFitting.FittedShip.ArmorExResist.ToString("N2") & "%</td><td width=20%>Kin: " & currentFitting.FittedShip.ArmorKiResist.ToString("N2") & "%</td><td width=20%>Th: " & currentFitting.FittedShip.ArmorThResist.ToString("N2") & "%</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Hull Resists</b></td><td width=20%>EM: " & currentFitting.FittedShip.StructureEMResist.ToString("N2") & "%</td> <td width=20%>Exp: " & currentFitting.FittedShip.StructureExResist.ToString("N2") & "%</td><td width=20%>Kin: " & currentFitting.FittedShip.StructureKiResist.ToString("N2") & "%</td><td width=20%>Th: " & currentFitting.FittedShip.StructureThResist.ToString("N2") & "%</td></tr>")
                        strHTML.Append("</table>")

                        ' Write capacitor stats
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr bgcolor=#111111><td colspan=4><b>Capacitor</b></td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Capacity</b></td><td width=30%>" & currentFitting.FittedShip.CapCapacity.ToString("N2") & " GJ</td>")
                        strHTML.Append("<td width=20%><b>Recharge</b></td><td width=30%>" & currentFitting.FittedShip.CapRecharge.ToString("N2") & " s</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Peak Recharge</b></td><td width=30%>" & (PluginSettings.HQFSettings.CapRechargeConstant * currentFitting.FittedShip.CapCapacity / currentFitting.FittedShip.CapRecharge).ToString("N2") & " GJ/s</td>")
                        strHTML.Append("<td width=20%><b>Stability</b></td><td width=30%>")
                        Dim csr As CapSimResults = Capacitor.CalculateCapStatistics(currentFitting.FittedShip, False)
                        If csr.CapIsDrained = False Then
                            strHTML.Append("Stable at " & (csr.MinimumCap / currentFitting.FittedShip.CapCapacity * 100).ToString("N2") & "%")
                        Else
                            strHTML.Append("Lasts " & SkillFunctions.TimeToString(csr.TimeToDrain, False))
                        End If
                        strHTML.Append("</td></tr>")
                        strHTML.Append("</table>")

                        ' Write Damage Stats
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr bgcolor=#111111><td colspan=4><b>Damage</b></td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Volley Damage</b></td><td width=30%>" & currentFitting.FittedShip.TotalVolley.ToString("N2") & "</td>")
                        strHTML.Append("<td width=20%><b>DPS</b></td><td width=30%>" & currentFitting.FittedShip.TotalDPS.ToString("N2") & "</td></tr>")
                        strHTML.Append("</table>")

                        ' Write Targeting Stats
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr bgcolor=#111111><td colspan=4><b>Targeting</b></td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Max Range</b></td><td width=30%>" & currentFitting.FittedShip.MaxTargetRange.ToString("N0") & " m</td>")
                        strHTML.Append("<td width=20%><b>Scan Resolution</b></td><td width=30%>" & currentFitting.FittedShip.ScanResolution.ToString("N2") & " mm</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Signature Radius</b></td><td width=30%>" & currentFitting.FittedShip.SigRadius.ToString("N0") & " m</td>")
                        strHTML.Append("<td width=20%><b>Max Targets</b></td><td width=30%>" & currentFitting.FittedShip.MaxLockedTargets.ToString("N0") & "</td></tr>")
                        strHTML.Append("</table>")

                        ' Write Propulsion Stats
                        strHTML.Append("<table width=100% border=1>")
                        strHTML.Append("<tr bgcolor=#111111><td colspan=4><b>Propulsion</b></td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Max Velocity</b></td><td width=30%>" & currentFitting.FittedShip.MaxVelocity.ToString("N2") & " m/s</td>")
                        strHTML.Append("<td width=20%><b>Warp Velocity</b></td><td width=30%>" & currentFitting.FittedShip.WarpSpeed.ToString("N2") & " au/s</td></tr>")
                        strHTML.Append("<tr bgcolor=#333333><td width=20%><b>Inertia</b></td><td width=30%>" & currentFitting.FittedShip.Inertia.ToString("N5") & "</td>")
                        strHTML.Append("<td width=20%><b>Warp Align Time</b></td><td width=30%>" & (-Math.Log(0.25) * currentFitting.FittedShip.Inertia * currentFitting.FittedShip.Mass / 1000000).ToString("N2") & " s</td></tr>")
                        strHTML.Append("</table>")

                        strHTML.Append("</td></tr>")
                        strHTML.Append("</table>")
                        strHTML.Append("Paste New Fitting: ")
                        strHTML.Append("<input type=""hidden"" name=""fitting"" value=""" & fittingURL & """>")
                        strHTML.Append("<input type=""text"" name=""fitting2"">")
                        strHTML.Append("<input type=""submit"" value=""Recalculate""></form>")

                        strHTML.Append("<form method=""GET"" action=""/EveHQFitter/SaveEveFitting"">")
                        strHTML.Append("Fitting Name: ")
                        strHTML.Append("<input type=""text"" name=""fittingname"" value=""" & fittingName & """>")
                        strHTML.Append("<input type=""submit"" value=""Save Fitting""></form>")

                    End If
                Catch e As Exception
                    strHTML.Append("<p>Unable to parse fitting: " & e.Message & "</p>")
                End Try
            End If
            Return strHTML.ToString
        End Function

        Shared Function SaveEveFitting(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder
            If context.Request.QueryString.Count = 0 Or HttpUtility.UrlDecode(context.Request.QueryString.Item("fittingname")) = "" Then
                strHTML.Append("<p>Fitting name is not permitted - go back and re-enter an appropriate name.</p>")
            Else
                Try
                    Dim fittingNameExists As Boolean = False
                    Dim fittingName As String = HttpUtility.UrlDecode(context.Request.QueryString.Item("fittingname"))
                    If Fittings.FittingList.ContainsKey(currentFitting.ShipName & ", " & fittingName) = True Then
                        fittingNameExists = True
                        Dim newFittingName As String
                        Dim revision As Integer = 1
                        Do
                            revision += 1
                            newFittingName = fittingName & " " & revision.ToString
                        Loop Until Fittings.FittingList.ContainsKey(currentFitting.ShipName & ", " & newFittingName) = False
                        fittingName = newFittingName
                    End If
                    Dim newFitting As Fitting = currentFitting.Clone
                    newFitting.FittingName = fittingName
                    newFitting.UpdateFittingFromBaseShip()
                    ' Let's save the fitting
                    Fittings.FittingList.Add(newFitting.KeyName, newFitting)
                    HQFEvents.StartUpdateFittingList = True
                    If fittingNameExists = False Then
                        strHTML.Append("<p>Fitting successfully saved as " & newFitting.FittingName & "!</p>")
                    Else
                        strHTML.Append("<p>The fitting '" & HttpUtility.UrlDecode(context.Request.QueryString.Item("fittingname")) & "' already exists, so the new fitting has been saved as '" & newFitting.FittingName & "'</p>")
                    End If
                    strHTML.Append("Click <a href=""javascript:history.go(-1)"">here</a> to go back to the fitting screen.")
                Catch ex As Exception
                    strHTML.Append("<p>There was an error saving the fitting. The error was: " & ex.Message & "<br /><br />" & ex.StackTrace & "</p>")
                End Try
            End If
            Return strHTML.ToString
        End Function

        Shared Function HQFPage2(ByVal context As HttpListenerContext) As String
            Dim strHTML As New StringBuilder
            Return strHTML.ToString
        End Function

    End Class
End Namespace