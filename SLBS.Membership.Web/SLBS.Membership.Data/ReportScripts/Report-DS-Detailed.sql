select CASE WHEN c.ClassLevel = 10 THEN 'Senior' ELSE 'Level ' + Convert (varchar(10),c.ClassLevel) End Class,
MembershipNumber, c.FullName as ChildName, 
CASE WHEN c.MediaConsent = 1 THEN 'YES' ELSE 'NO' END MemberConsent,
CASE WHEN c.AmbulanceCover = 1 THEN 'YES' ELSE 'NO' END AmbulanceCover,
LEFT(CONVERT(VARCHAR, PaidUpTo, 120), 10) PaymentStatus,
FatherName, MotherName, FatherEmail, FatherMobile, FatherLandphone,MotherEmail,MotherMobile,MotherLandphone

from Child c
inner join 
(select distinct Membership.MembershipId, Membership.MembershipNumber, Membership.PaidUpto, Father.FullName as FatherName, Mother.FullName as MotherName, 
Father.Email as FatherEmail, Father.MobilePhone as FatherMobile, Father.LandPhone as FatherLandphone, 
Mother.Email as MotherEmail, Mother.MobilePhone as MotherMobile, Mother.LandPhone as MotherLandphone
from Membership
left outer join
(select m.MembershipId, m.MembershipNumber, a.FullName, a.MobilePhone, a.Email, a.LandPhone
from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
where a.[Role] = 1) as Father
on Father.MembershipId = Membership.MembershipId
left outer join 
(select m.MembershipId, m.MembershipNumber, a.FullName,a.MobilePhone, a.Email,a.LandPhone
from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
where a.[Role] = 2) as Mother
on Mother.MembershipId = Membership.MembershipId) as MemberDetails
on MemberDetails.MembershipId = c.MembershipId 
order by ClassLevel, MembershipNumber
