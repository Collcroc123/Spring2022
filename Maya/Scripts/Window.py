import importlib
import maya.cmds as cmds
import colorsys as colorsys
import Tools
importlib.reload(Tools)


colorButton = 0


class ToolUI:
    def __init__(self):
        self.m_Window = "changeColorUIWin"
        self.width = 250
        self.height = 500
        self.color = (0, 0, 1)
        self.jntRadiusText = 0
        self.radiusText = 0
        self.hueText = 0
        self.sidesText = 0
        self.sweepText = 0
        self.nameText = 0

    def Create(self):
        global colorButton
        self.Delete()  # , s=False, mxb=False
        self.m_Window = cmds.window(self.m_Window, t="RIGGER", iconName="icon", wh=(self.width, self.height))
        cmds.columnLayout()
        cmds.button(l="TOGGLE ORIENTATION", c=lambda x: self.ToggleOrient(), bgc=(0.7, 0.7, 0.7), h=50, w=self.width, ann="Creates a locator on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.button(l="CREATE LOCATOR", c=lambda x: self.SetupLocator(), bgc=(0.7, 0.7, 0.7), h=50, w=self.width, ann="Creates a locator on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.button(l="CREATE JOINT", c=lambda x: self.SetupJoint(), bgc=(0.7, 0.7, 0.7), h=50, w=self.width, ann="Creates a joint on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.rowColumnLayout(nc=2, cat=(1, 'both', 0), cw=[(1, self.width / 2), (2, self.width / 2)])
        cmds.text(l='Radius', ann="The size of the Joint, accepts any float. 0 defaults to 1")
        self.jntRadiusText = cmds.textField(pht="Size", tx=1)
        cmds.text(l="\n")
        cmds.text(l="\n")
        cmds.columnLayout()
        cmds.button(l="CREATE CONTROL", c=lambda x: self.SetupControl(), bgc=(0.7, 0.7, 0.7), h=50, w=self.width, ann="Creates a control on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.rowColumnLayout(nc=2, cat=(1, 'both', 0), cw=[(1, self.width/2), (2, self.width/2)])
        cmds.text(l='Radius', ann="The size of the control, accepts any float. 0 defaults to 2")
        self.radiusText = cmds.textField(pht="Size", tx=2)
        cmds.text(l='Sides', ann="The number of sides the control has. 0 defaults to 30, a circle.")
        self.sidesText = cmds.textField(pht="3 or More", tx=30)
        cmds.text(l='Sweep', ann="The amount of the control drawn. 0 defaults to 360, a circle. A semicircle is 180.")
        self.sweepText = cmds.textField(pht="1 - 360 degrees", tx=360)
        cmds.text(l="\n")
        cmds.text(l="\n")
        cmds.button(l="Rename", c=lambda x: self.RenameSelected(), bgc=(0, 0.7, 0.2), h=25, w=self.width/2, ann="Renames selected object.")
        self.nameText = cmds.textField(pht="Name", tx="NAME_##_Jnt")
        cmds.text(l="\n")
        cmds.text(l="\n")
        colorButton = cmds.button(l="Color Picker", c=lambda x: self.ColorPick(), bgc=(1, 1, 1), h=25, w=self.width/2)
        cmds.button(l="Set Color", c=lambda x: Tools.ColorChange(self.color), bgc=(0.7, 0, 0.2), h=25, w=self.width/2)
        cmds.text(l="\n")
        cmds.text(l="\n")
        cmds.showWindow(self.m_Window)

    def Delete(self):
        if cmds.window(self.m_Window, exists=True):
            cmds.deleteUI(self.m_Window)

    def ToggleOrient(self):
        Tools.DisplayOrient()

    def SetupLocator(self):
        Tools.CreateLocator()

    def SetupJoint(self):
        jntRadiusNum = float(cmds.textField(self.jntRadiusText, q=True, tx=True))
        Tools.CreateJoint(jntRadiusNum)

    def SetupControl(self):
        radiusNum = float(cmds.textField(self.radiusText, q=True, tx=True))
        sidesNum = float(cmds.textField(self.sidesText, q=True, tx=True))
        sweepNum = float(cmds.textField(self.sweepText, q=True, tx=True))
        Tools.Control(radiusNum, self.color, sidesNum, sweepNum)

    def RenameSelected(self):
        nameText = cmds.textField(self.nameText, q=True, tx=True)
        Tools.Rename(nameText, 1)

    def ColorPick(self):
        self.color = Tools.ColorEdit()
        cmds.button(colorButton, edit=True, bgc=colorsys.hsv_to_rgb(self.color[0] / 360, self.color[1], self.color[2]))  # /360, 1, 0.9


ToolUI().Create()
