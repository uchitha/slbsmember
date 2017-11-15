select m.MembershipNumber, m.ContactName,LEFT(CONVERT(VARCHAR, m.PaidUpTo, 120), 10) PaidUpTo,LastNotificationDate,
                            Father.FullName as FathersName, Father.MobilePhone as FathersMobile, Father.Landphone as FathersLandphone, Father.Email as FathersEmail, COALESCE(Father.[Address],Mother.[Address]) as FamilyAddress,
                            Mother.FullName as MothersName, Mother.MobilePhone as MothersMobile, Mother.Landphone as MothersLandphone, Mother.Email as MothersEmail, 
							cast (CASE WHEN Child.ChildCount IS NULL THEN 0 ELSE 1 END as bit) as HasDsKids
                            from Membership m
                            left outer join 
                            (select m.MembershipId, m.MembershipNumber, a.FullName, a.MobilePhone, a.Email, a.LandPhone, a.[Address]
                            from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                            where a.[Role] = 1) as Father
                            on Father.MembershipId = m.MembershipId
                            left outer join 
                            (select m.MembershipId, m.MembershipNumber, a.FullName,a.MobilePhone, a.Email,a.LandPhone, a.[Address]
                            from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                            where a.[Role] = 2) as Mother
                            on Mother.MembershipId = m.MembershipId
							left outer join
							(select m.MembershipId, count(*) ChildCount from Child c inner join Membership m on c.MembershipId = m.MembershipId group by m.MembershipId ) as Child
							on Child.MembershipId = m.MembershipId

                            where m.MembershipNumber not like 'X%'
                            order by m.MembershipNumber