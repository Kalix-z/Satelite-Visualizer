from io import TextIOWrapper
import requests
import ephem
from ephem import degree

#ISS (ZARYA)             
# 25544U 98067A   22041.55493323  .00006763  00000+0  12771-3 0  9995
# 25544  51.6412 243.7057 0005670 116.5714 333.9560 15.49692082325546



def main() -> int:

    #dont want the old data
    clr_file();

    a = read_tle_data();

    i : int = 0;
    while (i < len(a)):
        lla : list = tle_to_latlongalt(a[i], a[i+1], a[i+2]);
        write_to_file(lla);
        i += 3;

    return 0;

def read_tle_data() -> list:
    returnVal : list = [];
    file = open("data.dat");
    lc : int = line_count("data.dat");

    for i in range(lc):
        returnVal.append(file.readline());
    
    file.close();
    return returnVal;
    



def tle_to_latlongalt(name : str, l1 : str, l2 : str) -> list:
    tle_rec : ephem.EarthSatellite = ephem.readtle(name, l1, l2);
    tle_rec.compute();

    lat : float  = tle_rec.sublat / degree;
    long : float = tle_rec.sublong / degree;
    alt : float = tle_rec.elevation;
    
    id : int = id_from_l2(l2);

    retVal : list = [];

    retVal.append(lat);
    retVal.append(long);
    retVal.append(alt);
    retVal.append(id);

    return retVal;

def id_from_l2(l2 : str) -> int:
    ls : list = l2.split(' ');
    return ls[1];

def write_to_file(data : list) -> None:
    file = open("latlongalt.dat", "a");
    s : str = format_data(data[0], data[1], data[2], data[3]);
    file.write(s);
    file.close();

def format_data(lat : float, long : float, alt : float, id : int) -> str:
    s : str = "";
    s += str(lat);
    s += ' ';
    s += str(long);
    s += ' ';
    s += str(alt);
    s += ' ';
    s += str(id);
    s += '\n';
    return s;

def line_count(fname) -> int:
    with open(fname) as f:
        for i, l in enumerate(f):
            pass
    return i + 1

def clr_file():
    file = open("latlongalt.dat", "w")
    file.close();
if __name__ == '__main__':
    main();