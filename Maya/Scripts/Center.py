import maya.cmds as cmds

def locator():
    sels = cmds.ls(sl=True)

    bbox = cmds.xform(sels, q=True, boundingBox=True, ws=True)
    midX = (bbox[0] + bbox[3]) / 2
    midY = (bbox[1] + bbox[4]) / 2
    midZ = (bbox[2] + bbox[5]) / 2

    loc = cmds.spaceLocator(position=[0,0,0], absolute=True) [0]

    cmds.xform(loc, translation=[midX, midY, midZ], ws=True)

locator()