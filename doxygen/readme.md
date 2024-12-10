# Doxygen Documentation
This document will walk you through how to generate documentation using doxyengine and our pre-setup doxyfile. We make use of [doxygen-awseome-css](https://github.com/jothepro/doxygen-awesome-css) repository from [jothepro](https://github.com/jothepro) to give it a pleasant and readable style.

## Install Doxygen
Firstly we need to install doxygen and use their doxywizard to start generating the code.

1. Go to [doxygen](https://www.doxygen.nl/) homepage and click the **Download** button in the top-right corner
2. Select the correct version for the pc platform you are on
3. Run the **"doxygen-setup.exe"** file and go throgh the installation proccess


## Generate Documentation
Now that we have it installed we can start generating the documentation.

1. Find the (FCVEngine.doxyfile)[FCVEngine.Doxyfile] inside (FCV-Engine/doxygen/)[..] and run it with **doxywizard**
2. It should already have all the setup you need so now you navigate to the **Run**-tab
3. Find the **Run doxygen** button and click it

Now the html documentation should be generated and it can be pushed to git so it becomes published to github-pages
