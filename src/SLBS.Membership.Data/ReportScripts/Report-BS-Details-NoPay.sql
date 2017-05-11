select m.MembershipNumber, m.ContactName,LEFT(CONVERT(VARCHAR, m.PaidUpTo, 120), 10) PaidUpTo,
                            Father.FullName as FathersName, Father.MobilePhone as FathersMobile, Father.Landphone as FathersLandphone, Father.Email as FathersEmail,
                            Mother.FullName as MotherName, Mother.MobilePhone as MothersMobile, Mother.Landphone as MothersLandphone, Mother.Email as MothersEmail
                            from Membership m
                            left outer join 
                            (select m.MembershipId, m.MembershipNumber, a.FullName, a.MobilePhone, a.Email, a.LandPhone
                            from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                            where a.[Role] = 1) as Father
                            on Father.MembershipId = m.MembershipId
                            left outer join 
                            (select m.MembershipId, m.MembershipNumber, a.FullName,a.MobilePhone, a.Email,a.LandPhone
                            from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                            where a.[Role] = 2) as Mother
                            on Mother.MembershipId = m.MembershipId

                            where m.MembershipNumber not like 'X%'
                            order by m.MembershipNumber