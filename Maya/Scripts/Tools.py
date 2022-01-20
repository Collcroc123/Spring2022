import maya.cmds as cmds
import colorsys as colorsys


def ColorChange(hue):
    sels = cmds.ls(sl=True)
    for sel in sels:
        shapes = cmds.listRelatives(sel, children=True, shapes=True)
        for shape in shapes:
            cmds.setAttr("%s.overrideEnabled" % shape, True)
            cmds.setAttr("%s.overrideRGBColors" % shape, True)
            color = colorsys.hsv_to_rgb(hue[0] / 360, hue[1], hue[2])  # / 360, 1, 0.7
            cmds.setAttr("%s.overrideColorRGB" % shape, color[0], color[1], color[2])
            print("Set selected object's color to " + color)
    return


def ColorEdit():
    cmds.colorEditor()
    if cmds.colorEditor(query=True, result=True):
        values = cmds.colorEditor(query=True, hsv=True)
        print("Selected Hue is " + str(values[0]))
        return values  # [0]


def Rename(name, startNum):
    objects = cmds.ls(sl=True)
    newObjs = []

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


def CreateLocator():
    sels = cmds.ls(sl=True)

    bbox = cmds.xform(sels, q=True, boundingBox=True, ws=True)
    midX = (bbox[0] + bbox[3]) / 2
    midY = (bbox[1] + bbox[4]) / 2
    midZ = (bbox[2] + bbox[5]) / 2

    loc = cmds.spaceLocator(position=[0,0,0], absolute=True) [0]

    cmds.xform(loc, translation=[midX, midY, midZ], ws=True)


def CreateJoint(rad):
    sels = cmds.ls(sl=True)

    for sel in sels:
        pos = cmds.xform(sel, q=True, rotatePivot=True, ws=True)

        cmds.select(cl=True)
        jnt = cmds.joint(position=[0, 0, 0], absolute=True)
        cmds.xform(jnt, translation=pos, ws=True)


def ParentJoints():
    sels = cmds.ls(sl=True)

    for i, sel in enumerate(sels):
        if i < (len(sels) - 1):
            cmds.parent(sels[i], sels[i + 1])


def DisplayOrient():
    sels = cmds.ls(sl=True)
    for sel in sels:
        if cmds.nodeType(sel) == 'joint':
            state = cmds.getAttr('%s.displayLocalAxis' % sel)
            cmds.setAttr('%s.displayLocalAxis' % sel, not state)
            cmds.setAttr('%s.displayLocalAxis' % sel, keyable=not state, channelBox=not state)
            cmds.setAttr('%s.jointOrientX' % sel, keyable=not state, channelBox=not state)
            cmds.setAttr('%s.jointOrientY' % sel, keyable=not state, channelBox=not state)
            cmds.setAttr('%s.jointOrientZ' % sel, keyable=not state, channelBox=not state)


def OrientJoints():
    return


def Control(rad, hue, sect, deg):
    objects = cmds.ls(sl=True)
    if objects:
        for item in objects:
            controlName = item + "_Ctrl"  # item.rpartition('_')[0]
            CreateControl(item, rad, hue, sect, deg, controlName)
    else:
        controlName = "Default_Ctrl"
        CreateControl(0, rad, hue, sect, deg, controlName)


def CreateControl(item, rad, hue, sect, deg, controlName):
    if rad == 0:
        rad = 2
    if sect == 0:
        sect = 30
    if deg == 0:
        deg = 360

    newControl = cmds.circle(c=(0, 0, 0), r=rad, s=sect, sw=deg, n=controlName, d=1)
    newControl = cmds.listRelatives(newControl)
    group = cmds.group(newControl, n=controlName + "_Grp")

    if item is not 0:
        cmds.matchTransform(group, item)

    cmds.setAttr(newControl[0] + ".overrideEnabled", 1)
    cmds.setAttr(newControl[0] + ".overrideRGBColors", 1)
    color = colorsys.hsv_to_rgb(hue[0] / 360, hue[1], hue[2])  # / 360, 1, 0.7
    cmds.setAttr(newControl[0] + ".overrideColorRGB", color[0], color[1], color[2])
