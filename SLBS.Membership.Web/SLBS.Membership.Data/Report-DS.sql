

--DS Class Breakdown
SELECT CASE WHEN c.ClassLevel = 10 THEN 'Senior' ELSE 'Level ' + Convert (varchar(10),c.ClassLevel) End Class,
c.FullName, xMember.MembershipNumber, xMember.Parents, xMember.Emails,xMember.MobilePhone,xMember.LandPhone
from Child c,
(SELECT
	m.MembershipId,
	m.MembershipNumber,
	STUFF((SELECT ', ' + a.FullName
	from Adult a
	Where a.MembershipId = m.MembershipId
	For XML PATH ('')),1,1,'' ) as Parents,
	STUFF((SELECT ',' + a.Email
	from Adult a
	Where a.MembershipId = m.MembershipId
	For XML PATH ('')),1,1,'' ) as Emails,
	STUFF((SELECT ', ' + a.MobilePhone
	from Adult a
	Where a.MembershipId = m.MembershipId
	For XML PATH ('')),1,1,'' ) as MobilePhone,
	STUFF((SELECT ', ' + a.LandPhone
	from Adult a
	Where a.MembershipId = m.MembershipId
	For XML PATH ('')),1,1,'' ) as LandPhone
from  Membership m
Group by m.MembershipId, m.MembershipNumber
) xMember
where xMember.MembershipId = c.MembershipId
order by ClassLevel, MembershipNumber





SELECT *,
                ROW_NUMBER() OVER (PARTITION BY MembershipNumber ORDER BY MembershipId) AS MFather,
                ROW_NUMBER() OVER (PARTITION BY MembershipNumber ORDER BY MembershipId DESC) AS MMother
         FROM   Membership
