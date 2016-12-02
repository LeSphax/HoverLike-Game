using System;
using System.Collections.Generic;
using UnityEngine;

public class Random_Name_Generator
{
    private static string[] lines;

    public static string GetRandomName()
    {
        if (lines == null)
        {
            lines = names.Replace("\r", "").Split('\n');
        }
        int r = UnityEngine.Random.Range(0, lines.Length - 1);
        return lines[r];
    }

    private const string names = @"Adam Baum
Adam Zapel
Al Bino
Al Dente
Al Fresco
Al K.Seltzer
Alf A. Romeo
Ali Gaither
Ali Katt
Amanda Lay
Amanda Lynn
Amber Green
Andy Friese
Anita Bathe
Anita Bohn
Anita Dick
Anita Friske
Anita Hanke
Anita Goodman
Anita Hoare
Anita Job
Anita Knapp
Anita Lay
Anita Little
Anita Mann
Anita Mandalay
Anita Plummer
Anna Conda
Anna Fender
Anna Graham
Anna Prentice
Anna Recksiek
Anna Sasin
Anne Teak
Annette Curtain
Annie Howe
Annie Matter
April May
April Schauer
Aretha Holly
Armand Hammer
Art Major
Art Painter
Art Sellers
B.A.Ware
Barb Dwyer
Barb E.Dahl
Barbara Seville
Barry Cade
Bea Minor and Dee Major
Beau Archer
Beau Tye
Ben Dover
Ben Down
Eileen Dover
Skip Dover
Ben Marcata
Bess Eaton
Biff Wellington
Bill Board
Bill Ding
Bill Foldes
Bill Loney
Billy Rubin
Bob Apple
Bob Katz
Tom Katz
Kitty Katz
Bonnie Ann Clyde
Bonnie Beaver
Brad Hammer
Brandon Cattell
Brandon Irons
Brandy Anne Koch
Brandy D. Cantor
Brighton Early
Brock Lee
Brooke Trout
Bud Light
Bud Wieser
Buster Cherry
Buster Hyman
C. Good
C. Senor
C. Worthy
C. Write
Cam Payne
Candace Spencer
Candy Barr
Candy Baskett
Candy Kane
Candy Sweet
Cara Sterio
Cara Van
Carrie Dababi
Carrie Oakey
Casey Macy
Charity Case
Cheri Pitts
Harry Pitts
Chip Munk
Chip Stone
Chris Coe
Chris Cross
Chris P.Bacon
Chuck U.Farley
Chuck Waggon
Claire Annette Reed
Constance Noring
Corey Ander
Corey O. Graff
Count Dunn
Count Orff
Coyne Flatt
Craven Moorehead
Crystal Ball
Crystal Claire Waters
Crystal Glass
Crystal Metheney
Crystal Snow
D.Kay
D. Liver
Dan D.Lyons
Dan Deline
Dan Druff
Dan Saul Knight
Darren Deeds
Daryl Rhea
Dear Beloved
Dick Bender
Dick Burns
Dick Bush
Dick Face
Dick Finder
Dick Head
Dick Hertz
Dick Hyman
Dick Hunter
Dick Long
Dick Mussell
Dick Pole
Dick Pound
Dick Rasch
Dick Swett
Dick Tator
Dick Trickle
Dick Wood
Dickson Yamada
Dilbert Pickles
Dinah Soares
Dixon
Cox
and Peters
Don Key
Donald Duck
Donny Brook
Doris Schutt
DooLittle & Dalley
Doug Graves
Doug Hole
Doug & Phil Updegrave
Doug Witherspoon
Douglas Furr
Dr.Baldock
Dr. Croak
Dr. Harry
C. Beaver
Dr. Bender
Dr. Butcher
Dr. DeKay
Dr. & Dr.Doctor
Dr. E.Ville
Dr. Shelly Fingerhood
Dr.Gass
Dr. Gutstein
Dr. Hanus
Dr. Hurt
Dr. Hymen
Dr. I.Ball
Dr. Kauff
Dr. Look
Dr. Looney
Dr. Payne
Dr. Pullham
Dr. Robert Fallis
Dr.Slaughter
Dr. Surgeon
Drew Peacock
Duane Pipe
Dusty Carr
Dusty Rhodes
Dusty Sandmann
Edna May
Earl E.Bird
Earl Lee Riser
Easton West
Eaton Wright and Liv Good
Edward Z.Filler
Ella Vader
Emma Royds
Eric Shinn
Ernie Coli
Estelle Hertz
Evan Keel
Faith Christian
Fanny O'Rear
Fanny Hertz
Father A. Long
Ferris Wheeler
Flint Sparks
Fonda Dicks
Ford Parker
Forrest Green
Foster Child
Dr. Frank Bonebreak
Frank Enstein
Dr.Franklin Stein
Gae Hooker
Gaye Barr
Gaye Jolly
Gail Force
Gail Storm
Gene Poole
Geoff L. Tavish
Gil Fish
Ginger Rayl
Ginger Snapp
Ginger Vitus
Gladys C.Hughes
H. Wayne Carver
Hamilton Burger
Harden Thicke
Harold Assman
Harry Armand Bach
Harry Baals
Harry Beard
Harry Beaver
Harry Butts
Harry Caray
Harry Chest
Harry Cox
Harry Dangler
Harry Johnson
Harry Legg
Harry Hooker
Harry P.Ness
Harry Peters
Harry Lipp
Harry Sachs
Harry R.M.Pitts
Harry Rump
Hazle Nutt
Heidi Clare
Helen Back
Helen Waite
Helen Wiells
Herb Farmer
Herb Rice
Holly McRell
Holly Day
Holly Wood
Honey Bee
Howie Doohan
Hugh Jass
Hugh Jorgan
Hugh Morris
Hy Ball
Hy Lowe
Bea Lowe
Hy Marx
Hy Price
I.D.Clair
I. Lasch
I.M.Boring
I.P.Freely
I.P.Daly
I. Pullem
Ileane Wright
Ilene South
Ima Hogg
Iona Ford
Iona Frisbee
Iona Stonehouse
Isadore Bell
Ivan Oder
Ivana Mandic
Ivy Leage
Jack Hoff
Jack Goff
Jack Haas
Jack Hammer
Jack Knoff
Jack Pott
Jack Tupp
Jacklyn Hyde
Jasmine Rice
Jay Walker
Jean Poole
Jed Dye
Jenny Tull
Jerry Atrick
Jim Laucher
Jim Shorts
Jim Shu
Jim Sox
Jo King
Joe Kerr
Jordan Rivers
Joy Kil
Joy Rider
June Bugg
Justin Case
Justin Casey Howells
Justin Hale
Justin Inch
Justin Miles North
Justin Sane
Justin Time
Kandi Apple
Katherine
Kay Bull
Keelan Early
Kelly Green
Ken Dahl
Kenny Penny
Kent C. Strait
Kenya Dewit
Kerry Oki
King Queene
Lake Speed
Lance Boyle
Lance Butts
Laura Lynne Hardy
Laurel Ann Hardy
Laura Norder
Laurence Getzoff
Leigh King
Les Moore
Les Payne
Les Plack
Levon Coates
Lewis N.Clark
Lily Pond
Lina Ginster
Lindsay Doyle
Lisa Carr
Kitty Carr
Otto Carr
Parker Carr
Lisa Ford
Lisa Honda
Iona Corolla
Lisa May Boyle
Lisa May Dye
Liv Long
Lois Price
Lou Pole
Lou Zar
Luckey
Lucy Fer
Luke Warm
Lynn C. Doyle
Lynn O.Liam
M. Balmer
Macon Paine
Mark Skid
Manny Kinn
Marlon Fisher
Marsha Dimes
Marsha Mellow
Marshall Law
Marty Graw
Mary Annette Woodin
Mary A. Richman
Mary Christmas
Matt Tress
Maude L.T.Ford
Max Little
Max Power
May Day
May Furst
Mel Loewe
Melba Crisp
Melody Music
Mia Hamm
Mike Easter
Mike Hunt
Mike Raffone
Mike Reinhart
Mike Rotch
Mike Stand
Mike Sweeney
Milly Graham
Minny van Gogh
Missy Sippy
Mister Bates
Misty Waters
Misty C. Shore
Mo Lestor
Moe B.Dick
Molly Kuehl
Mona Lott
Monica Monica
Morey Bund
Muddy Waters
Myles Long
Nancy Ann Cianci
Nat Sass
Neil Down
Neil Crouch
Neil McNeil
Nick O. Time
Noah Riddle
Noah Lott
Norma Leigh Lucid
Olive Branch
Olive Green
Olive Hoyl
Olive Yew
Oliver Sutton
Ophelia Payne
Oren Jellow
Orson Carte
Oscar Ruitt
Otto Graf
Owen Moore
Owen Bigg
P.Ness
A. Ness
P. Brain
Paige Turner
Park A.Studebaker
Pat Downe
Pat McCann
Pat Hiscock
Patience Wait
Pearl Button
Pearl E.Gates
Pearl E.White
Peg Legge
Penny Dollar
Bill Dollar
Penny Lane
Penny Nichols
Penny Profit
Penny Wise
Pepe Roni
Pete Moss
Peter Johnson
Dick Johnson
Peter Peed
Peter Wacko
Phil Bowles
Phil Graves
Phil Rupp
Phil Wright
Phillip D.Bagg
Pierce Cox
Pierce Deere
Pierce Hart
Polly Ester
Price Wright
Priti Manek
R. M.Pitt
R. Sitch
R. Slicker
Randy Guy
Randy Lover
Raney Schauer
Ray Gunn
Ray Zenz
Raynor Schein
Reid Enright
Rex Easley
Rhea Curran
Rhoda Booke
Rita Booke
Rich Feller
Rich Guy
Rich Kidd
Rich Mann
Richard P.Cox
Richard Chopp
Rick O'Shea
Rick Shaw
Rip Torn
Rita Buch
Rita Story
Robin Andis Merryman
Robin Banks
Rob Banks
Robin Feathers
Robin Money
U. O.Money
Robert Soles
Rock  Bottoms
Rock Pounder
Rock Stone
Rocky Beach
Sandy Beach
Rocky Mountain
Cliff Mountain
Rocky Rhoades
Rod N.Reel
Roman Holiday
Rose Bush
Rose Gardner
Rowan Boatman
Royal Payne
Russell Leeves
Russell Sprout
Rusty Blades
Rusty Bridges
Rusty Carr
Rusty Dorr
Rusty Fossat
Rusty Fender
Rusty Irons
Rusty Keyes
Rusty Nail
Rusty Pipes
Rusty Steele
Ryan Carnation
Ryan Coke
Sal A.Mander
Sal Minella
Sam Manilla
Sam & Ella
Sally Forth
Sarah Bellum
Sawyer B.Hind
Sawyer Dickey
Sandy Banks
Sandy Beech
Sandy Brown
Sandy Spring
Seth Poole
Seymour Bush
Shanda Lear
Sharon Fillerup
Sharon Needles
Sharon Weed
Sharon A.Burger
Sheila Blige
Skip Roper
Skip Stone
Sonny Day
Sno White
Stan Still
Stanley Cupp
Dr. Steven Sumey
Sue Flay
Sue Render
Sue Ridge
Sue Yu
Sue Jeu
Summer Camp
Summer Day
Summer Greene
Summer Holiday
Sy Burnette
Tad Moore
Tad Pohl
Tamara Knight
Tanya Hyde
Tara Cherry
Ted E. Baer
Terry Achey
Terry Bull
Tess Steckle
Therese R.Green
Teresa Green
Thomas Richard Harry
Tiffany Box
Tim Burr
Tish Hughes
Tittsworth & Grabbe
real law firm
Tom A. Toe
Tom Katt
Tom Morrow
Tommy Gunn
Tommy Hawk
Trina Woods
Trina Forest
Ty Coon
Ty Knotts
Urich Hunt
Viola Solo
Virginia Beach
Walter Melon
Wanda Rinn
Wanna Hickey
Warren Peace
Warren T.
Will Power
Will Race
Will Wynn
Willie B. Hardigan
Willie Leak
Willie Stroker
Willie Waite
Winsom Cash
Owen Cash
Woody Forrest
X. Benedict
";

}
