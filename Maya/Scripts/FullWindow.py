import maya.cmds as cmds
import colorsys as colorsys

colorButton = 0


class ToolUI:
    def __init__(self):
        self.m_Window = "changeColorUIWin"
        self.width = 120
        self.height = 500
        self.color = (0, 0, 1)
        self.jntRadiusText = 0
        self.radiusText = 0
        self.hueText = 0
        self.sidesText = 0
        self.sweepText = 0
        self.nameText = 0
        self.limbText = 0

    def Create(self):
        global colorButton
        self.Delete()
        self.m_Window = cmds.window(self.m_Window, t="Rigging", iconName="icon", nde=True, wh=(self.width, self.height))
        cmds.scrollLayout('scrollLayout')
        cmds.columnLayout(adjustableColumn=True)
        #cmds.columnLayout(cat=("both", -1))
        # ACTIONS
        cmds.frameLayout(label='Actions', labelAlign='center', cl=False, cll=True)
        cmds.columnLayout(adjustableColumn=True)
        cmds.button(l="Toggle Orient", c=lambda x: self.DisplayOrient(), bgc=(1, 0.4, 0.4), h=25, w=self.width, ann="Toggles view of orientation and orientation controls in Channel Box.")
        cmds.button(l="Freeze Transforms", c=lambda x: self.Freeze(), bgc=(1, 0.6, 0), h=25, w=self.width, ann="Freezes all transforms of selected objects.")
        cmds.button(l="Match Transforms", c=lambda x: self.Match(), bgc=(0.9, 0.9, 0), h=25, w=self.width, ann="Matches all transforms of selected objects.")
        cmds.button(l="Parent", c=lambda x: self.ParentJoints(), bgc=(0.4, 1, 0.4), h=25, w=self.width, ann="Parents objects in the order you selected them in.")
        cmds.button(l="Set Color", c=lambda x: self.ColorChange(self.color), bgc=(0.5, 0.6, 1), h=25, w=self.width)
        colorButton = cmds.button(l="Pick Color", c=lambda x: self.ColorPick(), bgc=(1, 1, 1), h=25, w=self.width)
        cmds.button(l="Rename", c=lambda x: self.Rename(1), bgc=(0.7, 0, 1), h=25, w=self.width, ann="Renames selected object.")
        cmds.text(l="\n", h=2)
        cmds.text(l='New Name', h=15, ann="The new name given to selection. ## are replaced with numbers")
        self.nameText = cmds.textField(pht="Name", tx="NAME_##_Jnt")
        cmds.text(l="\n", h=5)
        cmds.setParent('..')
        cmds.setParent('..')
        # CREATE
        cmds.frameLayout(label='Create', labelAlign='center', cl=True, cll=True)
        cmds.columnLayout(adjustableColumn=True)
        cmds.button(l="Locator", c=lambda x: self.CreateLocator(), bgc=(0.4, 1, 0.4), h=25, w=self.width, ann="Creates a locator on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.button(l="Joint", c=lambda x: self.CreateJoint(), bgc=(0.5, 0.6, 1), h=25, w=self.width, ann="Creates a joint on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.text(l="\n", h=2)
        cmds.text(l='Joint Radius', h=15, ann="The size of the Joint, accepts any float. 0 defaults to 1")  # fn="boldLabelFont",
        self.jntRadiusText = cmds.textField(pht="Size", tx=1)
        cmds.text(l="\n", h=5)
        cmds.button(l="Control", c=lambda x: self.SetupControl(), bgc=(1, 0.6, 0), h=25, w=self.width, ann="Creates a control on selected objects. When nothing is selected, it will create a default one at origin.")
        cmds.text(l="\n", h=2)
        cmds.text(l='Control Radius', h=15, ann="The size of the control, accepts any float. 0 defaults to 2")
        self.radiusText = cmds.textField(pht="Size", tx=2)
        cmds.text(l="\n", h=2)
        cmds.text(l='Control Sides', h=15, ann="The number of sides the control has. 0 defaults to 30, a circle.")
        self.sidesText = cmds.textField(pht="3 or More", tx=30)
        cmds.text(l="\n", h=2)
        cmds.text(l='Control Sweep', h=15, ann="Amount of control drawn. 0 defaults to 360 (circle). Semicircle = 180.")
        self.sweepText = cmds.textField(pht="1 - 360 degrees", tx=360)
        cmds.text(l="\n", h=5)
        cmds.setParent('..')
        cmds.setParent('..')
        # CONSTRAINTS
        cmds.frameLayout(label='Constraints', labelAlign='center', cl=True, cll=True)
        cmds.columnLayout(adjustableColumn=True)
        cmds.button(l="P-S Constraint", c=lambda x: self.Constraint(), bgc=(0.5, 0.6, 1), h=25, w=self.width, ann="Creates a constraint between selected Control & Joint.")
        cmds.button(l="Broken Constraint", c=lambda x: self.BrokenConstraint(), bgc=(0.9, 0.9, 0), h=25, w=self.width, ann="Creates a broken constraint between selected Control & Group.")
        cmds.setParent('..')
        cmds.setParent('..')
        # IK RIG
        cmds.frameLayout(label='IK Rig', labelAlign='center', cl=True, cll=True)
        cmds.columnLayout(adjustableColumn=True)
        cmds.button(l="Reverse Foot IK Attr", c=lambda x: self.ReverseFootIKAttr(), bgc=(1, 0.4, 0.4), h=25, w=self.width, ann="Creates Reverse Foot IK Attributes")
        cmds.button(l="Limb Twist", c=lambda x: self.LimbTwist(), bgc=(0.7, 0, 1), h=25, w=self.width, ann="Creates Limb Twists")
        cmds.text(l="\n", h=2)
        cmds.text(l='Limb Twist Name', h=15, ann="The name of the limb you're making a twist for")
        self.limbText = cmds.textField(pht="Limb", tx="L_Arm")
        cmds.text(l="\n", h=5)
        cmds.setParent('..')
        cmds.setParent('..')
        # NAME
        cmds.frameLayout(label='TEMPLATE', labelAlign='center', cl=True, cll=True)
        cmds.columnLayout(adjustableColumn=True)
        # STUFF GO HERE
        cmds.setParent('..')
        cmds.setParent('..')

        cmds.showWindow(self.m_Window)

    def Delete(self):
        if cmds.window(self.m_Window, exists=True):
            cmds.deleteUI(self.m_Window)

    def ColorPick(self):
        self.color = self.ColorEdit()
        cmds.button(colorButton, edit=True,
                    bgc=colorsys.hsv_to_rgb(self.color[0] / 360, self.color[1], self.color[2]))  # /360, 1, 0.9

    def ColorChange(self, hue):
        sels = cmds.ls(sl=True)
        for sel in sels:
            shapes = cmds.listRelatives(sel, children=True, shapes=True)
            for shape in shapes:
                cmds.setAttr("%s.overrideEnabled" % shape, True)
                cmds.setAttr("%s.overrideRGBColors" % shape, True)
                color = colorsys.hsv_to_rgb(hue[0] / 360, hue[1], hue[2])  # / 360, 1, 0.7
                cmds.setAttr("%s.overrideColorRGB" % shape, color[0], color[1], color[2])
                #print("Set selected object's color to " + color)
        return

    def ColorEdit(self):
        cmds.colorEditor()
        if cmds.colorEditor(query=True, result=True):
            values = cmds.colorEditor(query=True, hsv=True)
            print("Selected Hue is " + str(values[0]))
            return values  # [0]

    def Rename(self, startNum):
        objects = cmds.ls(sl=True)
        newObjs = []
        name = cmds.textField(self.nameText, q=True, tx=True)

        num_chars = name.count('#')
        name_parts = name.partition('#' * num_chars)

        # Is argument string correctly formatted?
        if not name_parts[2]:
            cmds.error('Argument string requires at least one "#" character.')

        # loop through each selected object
        for index, sel in enumerate(objects, start=startNum):
            newName = name_parts[0] + str(index).zfill(num_chars) + name_parts[2]
            newName = cmds.rename(sel, newName)
            newObjs.append(newName)

        return newObjs
# LOCATOR
    def CreateLocator(self):
        sels = cmds.ls(sl=True)

        bbox = cmds.xform(sels, q=True, boundingBox=True, ws=True)
        midX = (bbox[0] + bbox[3]) / 2
        midY = (bbox[1] + bbox[4]) / 2
        midZ = (bbox[2] + bbox[5]) / 2

        loc = cmds.spaceLocator(position=[0, 0, 0], absolute=True)[0]

        cmds.xform(loc, translation=[midX, midY, midZ], ws=True)
# JOINT
    def CreateJoint(self):
        sels = cmds.ls(sl=True)
        rad = float(cmds.textField(self.jntRadiusText, q=True, tx=True))
        if rad == 0:
            rad = 1
        for sel in sels:
            pos = cmds.xform(sel, q=True, rotatePivot=True, ws=True)
            cmds.select(cl=True)
            jnt = cmds.joint(position=[0, 0, 0], absolute=True, r=rad)
            cmds.xform(jnt, translation=pos, ws=True)

    def ParentJoints(self):
        sels = cmds.ls(sl=True)
        for i, sel in enumerate(sels):
            if i < (len(sels) - 1):
                cmds.parent(sels[i], sels[i + 1])

    def DisplayOrient(self):
        sels = cmds.ls(sl=True)
        for sel in sels:
            if cmds.nodeType(sel) == 'joint':
                state = cmds.getAttr('%s.displayLocalAxis' % sel)
                cmds.setAttr('%s.displayLocalAxis' % sel, not state)
                cmds.setAttr('%s.displayLocalAxis' % sel, keyable=not state, channelBox=not state)
                cmds.setAttr('%s.jointOrientX' % sel, keyable=not state, channelBox=not state)
                cmds.setAttr('%s.jointOrientY' % sel, keyable=not state, channelBox=not state)
                cmds.setAttr('%s.jointOrientZ' % sel, keyable=not state, channelBox=not state)
                
    def Freeze(self):
        sels = cmds.ls(sl=True)
        for sel in sels:
            cmds.makeIdentity(sel, apply=True, t=1, r=1, s=1, n=0)
        return
        
    def Match(self):
        sels = cmds.ls(sl=True)
        sels = cmds.ls(sl=True)
        for i, sel in enumerate(sels):
            if i < (len(sels) - 1):
                cmds.matchTransform(sels[i], sels[i + 1])
        return

    def OrientJoints(self):
        return

# CONTROLS
    def SetupControl(self):
        objects = cmds.ls(sl=True)
        rad = float(cmds.textField(self.radiusText, q=True, tx=True))
        hue = self.color
        sect = float(cmds.textField(self.sidesText, q=True, tx=True))
        deg = float(cmds.textField(self.sweepText, q=True, tx=True))
        if objects:
            for item in objects:
                controlName = item.replace("_Jnt", "_Ctrl")
                self.CreateControl(item, rad, hue, sect, deg, controlName)
        else:
            controlName = "Default_Ctrl"
            self.CreateControl(0, rad, hue, sect, deg, controlName)

    def CreateControl(self, item, rad, hue, sect, deg, controlName):
        if rad == 0:
            rad = 2
        if sect == 0:
            sect = 30
        if deg == 0:
            deg = 360

        newControl = cmds.circle(c=(0, 0, 0), r=rad, s=sect, sw=deg, n=controlName, d=1)
        cmds.rotate(0, 90, 0, r = True)
        newControl = cmds.listRelatives(newControl)
        grp = cmds.group(newControl, n=controlName + "_Grp")
        if item is not 0:
            cmds.matchTransform(grp, item)

        cmds.setAttr(newControl[0] + ".overrideEnabled", 1)
        cmds.setAttr(newControl[0] + ".overrideRGBColors", 1)
        color = colorsys.hsv_to_rgb(hue[0] / 360, hue[1], hue[2])
        cmds.setAttr(newControl[0] + ".overrideColorRGB", color[0], color[1], color[2])
    
    def Constraint(self):
        sels = cmds.ls(sl=True)
        parent = sels[0]
        child = sels[1]
        cmds.parentConstraint(parent, child, mo=True, w=1)
        cmds.scaleConstraint(parent, child, mo=True, w=1)
     
    def BrokenConstraint(self):
        sels = cmds.ls(sl=True)
        target = sels[0]
        ctrl = sels[1]
        grp = cmds.listRelatives(ctrl, p=True)[0]
        transCon = cmds.parentConstraint(target, grp, mo=True, sr=["x", "y", "z"], w=1)[0]
        rotCon = cmds.parentConstraint(target, grp, mo=True, st=["x", "y", "z"], w=1)[0]
        cmds.scaleConstraint(target, grp, mo=True, w=1)
        cmds.addAttr(ctrl, ln='FollowTranslate', at='double', min=0, max=1, dv=1)
        cmds.setAttr('%s.FollowTranslate' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='FollowRotate', at='double', min=0, max=1, dv=1) # parent=cmds.listRelatives(c=True)[1]
        cmds.setAttr('%s.FollowRotate' % ctrl, e=True, keyable=True)
        cmds.connectAttr('%s.FollowTranslate' % ctrl, '%s.w0' % transCon, f=True)
        cmds.connectAttr('%s.FollowRotate' % ctrl, '%s.w0' % rotCon, f=True)
        
    def ReverseFootIKAttr(self):
        sels = cmds.ls(sl=True)
        ctrl = sels[0]
        heel = sels[1]
        toe = sels[2]
        tap = sels[3]
        ball = sels[4]
        cmds.addAttr(ctrl, ln='HeelTap', at='float', dv=0)
        cmds.setAttr('%s.HeelTap' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='HeelRoll', at='float', dv=0)
        cmds.setAttr('%s.HeelRoll' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='HeelTwist', at='float', dv=0)
        cmds.setAttr('%s.HeelTwist' % ctrl, e=True, keyable=True)
        cmds.connectAttr('%s.HeelTap' % ctrl, '%s.rotate.rotateZ' % heel, f=True)
        cmds.connectAttr('%s.HeelRoll' % ctrl, '%s.rotate.rotateX' % heel, f=True)
        cmds.connectAttr('%s.HeelTwist' % ctrl, '%s.rotate.rotateY' % heel, f=True)
        
        cmds.addAttr(ctrl, ln='ToeTap', at='float', dv=0)
        cmds.setAttr('%s.ToeTap' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='ToeRoll', at='float', dv=0)
        cmds.setAttr('%s.ToeRoll' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='ToeTwist', at='float', dv=0)
        cmds.setAttr('%s.ToeTwist' % ctrl, e=True, keyable=True)
        cmds.connectAttr('%s.ToeTap' % ctrl, '%s.rotate.rotateZ' % toe, f=True)
        cmds.connectAttr('%s.ToeRoll' % ctrl, '%s.rotate.rotateX' % toe, f=True)
        cmds.connectAttr('%s.ToeTwist' % ctrl, '%s.rotate.rotateY' % toe, f=True)
        
        cmds.addAttr(ctrl, ln='TipTap', at='float', dv=0)
        cmds.setAttr('%s.TipTap' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='TipRoll', at='float', dv=0)
        cmds.setAttr('%s.TipRoll' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='TipTwist', at='float', dv=0)
        cmds.setAttr('%s.TipTwist' % ctrl, e=True, keyable=True)
        cmds.connectAttr('%s.TipTap' % ctrl, '%s.rotate.rotateZ' % tap, f=True)
        cmds.connectAttr('%s.TipRoll' % ctrl, '%s.rotate.rotateX' % tap, f=True)
        cmds.connectAttr('%s.TipTwist' % ctrl, '%s.rotate.rotateY' % tap, f=True)
        
        cmds.addAttr(ctrl, ln='BallTap', at='float', dv=0)
        cmds.setAttr('%s.BallTap' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='BallRoll', at='float', dv=0)
        cmds.setAttr('%s.BallRoll' % ctrl, e=True, keyable=True)
        cmds.addAttr(ctrl, ln='BallTwist', at='float', dv=0)
        cmds.setAttr('%s.BallTwist' % ctrl, e=True, keyable=True)
        cmds.connectAttr('%s.BallTap' % ctrl, '%s.rotate.rotateZ' % ball, f=True)
        cmds.connectAttr('%s.BallRoll' % ctrl, '%s.rotate.rotateX' % ball, f=True)
        cmds.connectAttr('%s.BallTwist' % ctrl, '%s.rotate.rotateY' % ball, f=True)
        
        cmds.addAttr(ctrl, ln='FootRoll', at='float', min=-10, max=10, dv=0)
        cmds.setAttr('%s.FootRoll' % ctrl, e=True, keyable=True)
        #cmds.connectAttr('%s.BallTap' % ctrl, '%s.rotate.rotateZ' % ball, f=True)
        #Connected L_Leg_IK_Ctrl.HeelTap to L_Heel_IK_Reverse_Jnt.rotate.rotateX
        
    def LimbTwist(self):
        sels = cmds.ls(sl=True)
        base = sels[0]
        first = sels[1]
        second = sels[2]
        third = sels[3]
        fourth = sels[4]
        limb = cmds.textField(self.limbText, q=True, tx=True)
        # REPLACE LINE ABOVE WITH limb = "L_Arm" or "R_Leg" etc
        # LOWER LIMB
        grp = cmds.group(em=True, n=limb+"_Lower_Twist_Grp")
        cmds.matchTransform(grp, third)
        target = cmds.spaceLocator(n=limb+"_Twist_Target_LOC")
        cmds.matchTransform(target, second)
        cmds.parent(target, grp)
        aim = cmds.spaceLocator(n=limb+"_Twist_Aim_LOC")
        cmds.matchTransform(aim, third)
        cmds.parent(aim, grp)
        up = cmds.spaceLocator(n=limb+"_Twist_Up_LOC")
        cmds.matchTransform(up, third)
        cmds.parent(up, grp)
        if 'Leg' in limb:
            if 'L_' in limb:
                cmds.move(10, 0, 0, r=True)
            else:
                cmds.move(-10, 0, 0, r=True)
        else:
            cmds.move(0, 0, 10, r=True)
        cmds.aimConstraint(target, aim, o=[0,0,0], w=1, aim=[-1,0,0], u=[0,1,0], wut="object", wuo=limb+"_Twist_Up_LOC")
        twist = cmds.spaceLocator(n=limb+"_Twist_LOC")
        cmds.matchTransform(twist, third)
        cmds.parent(twist, grp)
        cmds.pointConstraint(target, aim, twist, o=[0,0,0], w=1)
        md = cmds.shadingNode('multiplyDivide', au=True, n=limb+"_Lower_Twist_MD")
        if 'R_' in limb:
            cmds.setAttr('%s.input2X' % md, -0.5)
        else:
            cmds.setAttr('%s.input2X' % md, 0.5)
        cmds.connectAttr('%s.rotateX' % aim[0], '%s.input1X' % md, f=True)
        cmds.connectAttr('%s.outputX' % md, '%s.rotateX' % twist[0], f=True)
        cmds.pointConstraint(fourth, aim, mo=True, w=1)
        cmds.parentConstraint(fourth, up, mo=True, w=1)
        cmds.parentConstraint(second, grp, mo=True, w=1)
        cmds.scaleConstraint(second, grp, mo=True, w=1)
        jntOne = cmds.joint(p=[0, 0, 0], n=limb+"_Lower_Twist_01_Jnt", a=True, r=0.25)
        cmds.matchTransform(jntOne, third)
        cmds.parent(jntOne, second)
        cmds.parentConstraint(twist, jntOne, w=1)
        jntTwo = cmds.joint(p=[0, 0, 0], n=limb+"_Lower_Twist_02_Jnt", a=True, r=0.25)
        cmds.matchTransform(jntTwo, third)
        cmds.parent(jntTwo, second)
        cmds.parentConstraint(aim, jntTwo, w=1)
        # UPPER LIMB
        grp = cmds.group(em=True, n=limb + "_Upper_Twist_Grp")
        cmds.matchTransform(grp, first)
        target = cmds.spaceLocator(n=limb + "_Upper_Twist_Target_LOC")
        cmds.matchTransform(target, second)
        cmds.parent(target, grp)
        aim = cmds.spaceLocator(n=limb + "_Upper_Twist_Aim_LOC")
        cmds.matchTransform(aim, first)
        cmds.parent(aim, grp)
        up = cmds.spaceLocator(n=limb + "_Upper_Twist_Up_LOC")
        cmds.matchTransform(up, second)
        cmds.parent(up, grp)
        if 'Leg' in limb:
            if 'L_' in limb:
                cmds.move(10, 0, 0, r=True)
            else:
                cmds.move(-10, 0, 0, r=True)
        else:
            cmds.move(0, 0, -10, r=True)
        cmds.aimConstraint(target, aim, o=[0, 0, 0], w=1, aim=[1, 0, 0], u=[0, -1, 0], wut="object", wuo=limb + "_Upper_Twist_Up_LOC")
        twist = cmds.spaceLocator(n=limb + "_Upper_Twist_LOC")
        cmds.matchTransform(twist, first)
        cmds.parent(twist, grp)
        cmds.pointConstraint(target, aim, twist, o=[0, 0, 0], w=1)
        md = cmds.shadingNode('multiplyDivide', au=True, n=limb + "_Upper_Twist_MD")
        if 'R_' in limb:
            cmds.setAttr('%s.input2X' % md, -0.5)
        else:
            cmds.setAttr('%s.input2X' % md, 0.5)
        cmds.connectAttr('%s.rotateX' % aim[0], '%s.input1X' % md, f=True)
        cmds.connectAttr('%s.outputX' % md, '%s.rotateX' % twist[0], f=True)
        cmds.pointConstraint(second, target, mo=True, w=1)
        cmds.parentConstraint(first, grp, mo=True, w=1)
        cmds.scaleConstraint(first, grp, mo=True, w=1)
        # IK Upper Stuff
        ikgrp = cmds.group(em=True, n=limb + "_Upper_IK_Twist_Grp")
        cmds.matchTransform(ikgrp, first)
        if 'Leg' in limb:
            if 'L_' in limb:
                cmds.move(10, 0, 0, r=True)
            else:
                cmds.move(-10, 0, 0, r=True)
        else:
            cmds.move(0, 0, -10, r=True)
        ikjntOne = cmds.joint(p=[0, 0, 0], n=limb + "_Upper_IK_Twist_01_Jnt", a=True, r=0.25)
        cmds.matchTransform(ikjntOne, ikgrp)
        #cmds.parent(ikjntOne, ikgrp)
        ikjntTwo = cmds.joint(p=[0, 0, 0], n=limb + "_Upper_IK_Twist_02_Jnt", a=True, r=0.25)
        cmds.matchTransform(ikjntTwo, ikgrp)
        #cmds.parent(ikjntTwo, ikjntOne)
        cmds.move(6, 0, 0, r=True, ls=True)
        ikup = cmds.spaceLocator(n=limb + "_Upper_IK_Twist_Up_LOC")
        cmds.matchTransform(ikup, ikgrp, pos=True)
        cmds.parent(ikup, ikgrp)
        if 'Leg' in limb:
            cmds.move(0, 5, 0, ls=True)
        else:
            cmds.move(0, 0, -5, ls=True)
        handle = cmds.ikHandle(sj=ikjntOne, ee=ikjntTwo, n=limb + "_Upper_IK_Twist_Handle", shf=False)
        cmds.matchTransform(handle, second, pos=True)
        cmds.poleVectorConstraint(ikup, limb + "_Upper_IK_Twist_Handle", weight=1)
        cmds.makeIdentity(ikjntOne, apply=True, t=1, r=1, s=1, n=0)
        cmds.delete(handle)
        handle = cmds.ikHandle(sj=ikjntOne, ee=ikjntTwo, n=limb + "_Upper_IK_Twist_Handle", shf=False)
        cmds.matchTransform(handle, second, pos=True)
        cmds.poleVectorConstraint(ikup, limb + "_Upper_IK_Twist_Handle", weight=1)
        cmds.pointConstraint(second, limb + "_Upper_IK_Twist_Handle", mo=True, w=1)
        cmds.parent(limb + "_Upper_IK_Twist_Handle", ikgrp)
        cmds.parentConstraint(base, ikgrp, mo=True, w=1)
        cmds.scaleConstraint(base, ikgrp, mo=True, w=1)
        cmds.parentConstraint(ikjntOne, up, mo=True, w=1)
        jntOne = cmds.joint(p=[0, 0, 0], n=limb + "_Upper_Twist_01_Jnt", a=True, r=0.25)
        cmds.matchTransform(jntOne, first)
        cmds.parent(jntOne, first)
        cmds.parentConstraint(aim, jntOne, w=1)
        jntTwo = cmds.joint(p=[0, 0, 0], n=limb + "_Upper_Twist_02_Jnt", a=True, r=0.25)
        cmds.matchTransform(jntTwo, first)
        cmds.parent(jntTwo, first)
        cmds.parentConstraint(twist, jntTwo, w=1)

ToolUI().Create()
