import maya.cmds as cmds

def createLocator():
    sels = cmds.ls(sl=True)

    bbox = cmds.xform(sels, q=True, boundingBox=True, ws=True)
    midX = (bbox[0] + bbox[3]) / 2
    midY = (bbox[1] + bbox[4]) / 2
    midZ = (bbox[2] + bbox[5]) / 2

    loc = cmds.spaceLocator(position=[0,0,0], absolute=True) [0]

    cmds.xform(loc, translation=[midX, midY, midZ], ws=True)


def createJoint():
    sels = cmds.ls(sl=True)
    
    for sel in sels:
        pos = cmds.xform(sel, q=True, rotatePivot=True, ws=True)
        
        cmds.select(cl=True)
        jnt = cmds.joint(position=[0,0,0], absolute=True)
        cmds.xform(jnt, translation=pos, ws=True)
        
        
def parentJoints():
    sels = cmds.ls(sl=True)
    
    for i, sel in enumerate(sels):
        if i < (len(sels)-1):
            cmds.parent(sels[i], sels[i+1])
            
            

parentJoints()