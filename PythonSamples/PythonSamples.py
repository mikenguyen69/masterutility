##1. String 

#print("he said \"that's some greating apple\"")

#vegetable = 'broccoli'
#print(vegetable.upper())

#print('-'*12)

#version = 3
#print ('Python ' + str(version) + ' is fun.')

#print ('Python {0} {1} and {1} {0} awesome!'.format('is', 'fun'))

##< for left, ^ for center, and > for right
#print('{0:9} | {1:<8}'.format('Vegetable', 'Quantity')) 
#print('{0:9} | {1:<8}'.format('Asparagus', 3)) 
#print('{0:9} | {1:<8}'.format('Onions', 10))

##user_input = input('Please enter something and press enter: ')
##print('you entered:')
##print(user_input)

##2. Numbers, Math, Comment
#print('Cost is ${:.2f}'.format(20.33333))

##3. Boolean and Conditionals
#count = 3
#if count >= 2 and count <= 5 :
#    print('awsome!')


##4. List
##bikes = ['trek','giant','redline'] 
#bikes = []
#bikes.append('trek')
#bikes.append('giant')
#bikes.append('redline')

#first_bike = bikes[0]
#last_bike = bikes[-1]

#print('First bike {} and last bike {}'.format(first_bike, last_bike))

#for bike in bikes: 
#    print(bike)

##squares = []
##for x in range(1,11):
##    squares.append(x**2)

#for s in [x**2 for x in range(1,11)]: 
#    print(s)

#copy_of_bikes = bikes[:]
#for copy in copy_of_bikes: 
#    print(copy)

#first_two = bikes[:2]
#for b in first_two:
#    print(b)


##5. Turple - Similar to List but can't be modified 
#dimension = (1,5,10,15,20)
#for d in dimension:
#    print(d)

##6 Dictionary - stores connection between information. key - value pair
#alien = {'color': 'green', 'points' : 5}

#print ("the alien's colour is " + alien['color'])

#for name, value in alien.items(): #alien.keys(), alien.values()
#    print (name + ' loves ' + str(value))

##7 Loop
#current_value = 1
#while current_value <= 5:
#    print(current_value)
#    current_value +=1

##msg = ''
##while msg != 'quit':
##    msg = input("what is your message?")
##    print(msg)


##8 Functions
#def great_user(name):
#    print("Greeting " + name)

#great_user("Mike")


def make_pizza(topping="bacon"):
    print("Have a " + topping + " pizza!")


make_pizza()
make_pizza('peperonin')

##9 Class 
#class Dog(): 

#    def __init_(self, name): 
#        self.name = name







 